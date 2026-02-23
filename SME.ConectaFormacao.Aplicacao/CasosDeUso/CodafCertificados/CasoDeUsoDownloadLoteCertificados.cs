using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;
using SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System.Text;
using System.Threading.Channels;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoDownloadLoteCertificados(
        IRepositorioCodafCertificado repositorio,
        IServicoArmazenamento servicoArmazenamento,
        IServicoCompactacao servicoCompactacao,
        IServicoRelatorio servicoRelatorio) : ICasoDeUsoDownloadLoteCertificados
    {
        public async Task ExecutarAsync(List<long> ids, Stream streamSaida, CancellationToken cancellationToken = default)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("A lista de IDs não pode ser vazia.", nameof(ids));
            var certificados = await repositorio.ObterCertificadosDisponiveisPorListaDeIdAsync(ids);

            if (!certificados.Any())
                throw new InvalidOperationException("Nenhum certificado encontrado para os IDs informados.");

            var limiteArquivosEmMemoria = 10;
            var maximoTarefasSimultaneas = 5; // Throttling
            var opcoesCanal = new BoundedChannelOptions(capacity: limiteArquivosEmMemoria)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            var canal = Channel.CreateBounded<ArquivoCompactacaoDto>(opcoesCanal);

            var producerTask = Task.Run(async () =>
            {
                try
                {
                    var opcoesParalelismo = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = maximoTarefasSimultaneas,
                        CancellationToken = cancellationToken
                    };

                    await Parallel.ForEachAsync(certificados, opcoesParalelismo, async (certificado, ct) =>
                    {
                        var arquivoDto = await ProcessarCertificadoAsync(certificado, ct);
                        await canal.Writer.WriteAsync(arquivoDto, ct);
                    });

                    canal.Writer.Complete();
                }
                catch (Exception ex)
                {
                    canal.Writer.Complete(ex);
                }
            }, cancellationToken);

            var fluxoArquivos = canal.Reader.ReadAllAsync(cancellationToken);

            var fluxoTratado = fluxoArquivos.TratarNomesDuplicados(cancellationToken);

            await servicoCompactacao.CompactarAssincronamenteAsync(fluxoTratado, streamSaida, cancellationToken);

            await producerTask;
        }

        private async Task<ArquivoCompactacaoDto> ProcessarCertificadoAsync(CodafCertificado certificado, CancellationToken cancellationToken)
        {
            var nomeArquivo = $"CERTIFICADO_{certificado.CodigoCertificado:D4}_{certificado.DataEmissao:ddMMyyyy}.pdf";
            if (!string.IsNullOrEmpty(certificado.ChaveObjetoArmazenamento))
            {
                var storageResultado = await servicoArmazenamento.ObterArquivoPorChaveAsync(certificado.ChaveObjetoArmazenamento, cancellationToken);
                if (storageResultado.Sucesso)
                    return new(nomeArquivo, storageResultado.Dados!);
            }

            var pdfResultado = await servicoRelatorio.ConveterHtmlCertificadoCodafParaPdfAsync(new HtmlCertificadoCodafDto
            {
                HtmlContent = certificado.HtmlContentSnapshot
            }, cancellationToken);
            if (pdfResultado.Sucesso)
                return new(nomeArquivo, pdfResultado.Dados!);

            var mensagemErro = $"Erro ao gerar certificado N° {certificado.CodigoCertificado:D4}\n\nERRO:\n{string.Join("\n - ", pdfResultado.MensagensErro)}";
            var conteudoStream = new MemoryStream(Encoding.UTF8.GetBytes(mensagemErro));
            nomeArquivo = $"ERRO_CERTIFICADO_{certificado.CodigoCertificado:D4}_{certificado.DataEmissao:ddMMyyyy}.txt";
            return new ArquivoCompactacaoDto(nomeArquivo, conteudoStream);
        }
    }
}
