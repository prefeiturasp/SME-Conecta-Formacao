using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoGerarArquivoDeclaracoesCodaf(
        IServicoRelatorio servicoRelatorio,
        IRepositorioCodafDeclaracao repositorioCodafDeclaracao,
        IServicoArmazenamento servicoArmazenamento,
        IKeyedServiceProvider serviceProvider,
        IConfiguration configuration,
        UtilitariosCodaf utilitarios) :
        ICasoDeUsoGerarArquivoDeclaracoesCodaf
    {
        private readonly UtilitariosCodaf _utilitarios = utilitarios;

        public async Task<bool> Executar(MensagemRabbit param)
        {
            await _utilitarios.SalvarLogAsync("Início do processamento de declarações Codaf");
            var temDeclaracoesParaProcessar = true;

            var urlFrontEnd = configuration["UrlFrontEnd"];
            var urlAcessoDeclaracoes = $"{urlFrontEnd?.TrimEnd('/')}/declaracoes";
            await _utilitarios.SalvarLogAsync($"Url de acesso aos declaracoes: {urlAcessoDeclaracoes}");

            while (temDeclaracoesParaProcessar)
            {
                var notificacoesParaEnviar = new List<EnviarEmailDto>();
                var loteDeclaracoes = await repositorioCodafDeclaracao.ObterDeclaracoesParaProcessamentoAsync();
                if (!loteDeclaracoes.Any())
                {
                    temDeclaracoesParaProcessar = false;
                    continue;
                }
                var declaracoesProcessadas = await ProcessarLoteAsync(loteDeclaracoes);

                foreach (var declaracao in declaracoesProcessadas)
                {
                    var tipoEstrategia = UtilitariosCodaf.DefinirEstrategia(declaracao);
                    var geradorDeclaracao = serviceProvider.GetRequiredKeyedService<IDeclaracaoCodafGeradorConteudo>(tipoEstrategia);

                    var (tituloEmail, textoEmail) = geradorDeclaracao.GerarConteudoEmail(declaracao, urlAcessoDeclaracoes);

                    if (!string.IsNullOrEmpty(declaracao.EmailUsuario))
                    {
                        notificacoesParaEnviar.Add(new()
                        {
                            EmailDestinatario = declaracao.EmailUsuario!,
                            NomeDestinatario = declaracao.NomeCompleto,
                            Texto = textoEmail,
                            Titulo = tituloEmail
                        });
                    }
                }

                _ = _utilitarios.EnviarEmailsAsync(notificacoesParaEnviar);
            }
            await _utilitarios.SalvarLogAsync("Fim do processamento de declarações Codaf");

            return true;
        }

        private async Task<List<DadosProcessamentoCodafDto>> ProcessarLoteAsync(IEnumerable<DadosProcessamentoCodafDto> codafDeclaracoes)
        {
            var declaracoesProcessadas = new List<DadosProcessamentoCodafDto>();

            foreach (var declaracao in codafDeclaracoes)
            {
                try
                {
                    var htmlComSequencial = StringExtensao.InserirSequencialNoHtml(declaracao.HtmlContentSnapshot, declaracao.CodigoDeclaracaoOuCertificado);
                    var htmlComSigla = StringExtensao.InserirEmissor(htmlComSequencial, declaracao.Emissor);
                    var htmlDeclaracaoDto = new HtmlCodafDto
                    {
                        HtmlContent = htmlComSigla
                    };
                    var arquivoPdf = await servicoRelatorio.ConveterHtmlCodafParaPdfAsync(htmlDeclaracaoDto);
                    var declaracaoIdGuid = Guid.NewGuid();
                    var nomeDoArquivo = $"{DateTime.Now:yyyy/MM}/{declaracao.CodigoDeclaracaoOuCertificado}-{declaracaoIdGuid}.pdf";
                    var chaveObjetoArmazenamento = await servicoArmazenamento.UploadCodafAsync(nomeDoArquivo, arquivoPdf);
                    await repositorioCodafDeclaracao.AtualizarStatusProcessamentoAsync(
                        declaracao.Id,
                        StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso,
                        chaveObjetoArmazenamento,
                        null);
                    declaracoesProcessadas.Add(declaracao);
                }
                catch (Exception e)
                {
                    await _utilitarios.SalvarLogAsync($"Erro ao processar declaração Codaf com Id {declaracao.Id} e Código {declaracao.CodigoDeclaracaoOuCertificado}: {e.Message}", LogNivel.Critico, e);
                    await repositorioCodafDeclaracao.AtualizarStatusProcessamentoAsync(declaracao.Id, StatusProcessamentoDeclaracaoCodaf.ProcessadoComErro, null, e.Message);
                }
            }
            return declaracoesProcessadas;
        }
    }
}
