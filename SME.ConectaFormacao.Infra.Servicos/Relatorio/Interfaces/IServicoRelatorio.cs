using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces
{
    public interface IServicoRelatorio
    {
        Task<string> ObterRelatorioPropostaLaudaDePublicacao(long propostaId);
        Task<string> ObterRelatorioPropostaLaudaCompleta(long propostaId);
        Task<byte[]> ConveterHtmlCertificadoCodafParaPdfAsync(HtmlCertificadoCodafDto htmlCertificadoCodafDto);
        Task<Resultado<Stream>> ConveterHtmlCertificadoCodafParaPdfAsync(
            HtmlCertificadoCodafDto htmlCertificadoCodafDto,
            CancellationToken cancellationToken);
        Task<byte[]> GerarRelatorioCodafAsync(long codafId);
    }
}
