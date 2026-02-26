using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados
{
    public class CasoDeUsoObterCertificadoCodafParaDownload(
        IRepositorioCodafCertificado repositorioCodafCertificado,
        IServicoArmazenamento servicoArmazenamento) : ICasoDeUsoObterCertificadoCodafParaDownload
    {
        public async Task<Resultado<CodafCertificadoParaDownloadDto>> ExecutarAsync(long certificadoCodafId)
        {
            var certificadoCodaf = await repositorioCodafCertificado.ObterCertificadoDisponivelDoUsuarioAsync(certificadoCodafId);
            if (certificadoCodaf == null)
                return Erro.NaoEncontrado("Certificado CODAF não encontrado para o ID informado.");
            if (string.IsNullOrEmpty(certificadoCodaf.ChaveObjetoArmazenamento))
                return Erro.Validacao("Certificado CODAF não possui arquivo associado para download.");
            var urlArquivo = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(certificadoCodaf.ChaveObjetoArmazenamento);
            if (urlArquivo == null)
                return Erro.NaoEncontrado("Não foi possível obter o arquivo do certificado CODAF.");

            var arquivoDto = new CodafCertificadoParaDownloadDto
            {
                CodigoCertificado = certificadoCodaf.CodigoCertificado,
                Id = certificadoCodaf.Id,
                UrlDownload = urlArquivo,
                NomeCompleto = certificadoCodaf.NomeCompleto,
                NomeFormacao = certificadoCodaf.NomeFormacao
            };
            return arquivoDto;
        }
    }
}
