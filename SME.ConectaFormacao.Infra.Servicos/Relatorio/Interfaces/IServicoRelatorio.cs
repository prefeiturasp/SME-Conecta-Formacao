using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces
{
    public interface IServicoRelatorio
    {
        Task<string> ObterRelatorioPropostaLaudaDePublicacao(long propostaId);
        Task<string> ObterRelatorioPropostaLaudaCompleta(long propostaId);
        Task<byte[]> ConveterHtmlCodafParaPdfAsync(HtmlCodafDto htmlCertificadoCodafDto);
        Task<Resultado<Stream>> ConveterHtmlCodafParaPdfAsync(
            HtmlCodafDto htmlCertificadoCodafDto,
            CancellationToken cancellationToken);
        Task<byte[]> GerarRelatorioCodafAsync(long codafId);
        Task<byte[]> GerarRelatorioCodafSuplementarAsync(long codafListaPresencaId);
    }
}
