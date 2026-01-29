using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafCertificado
    {
        Task<IEnumerable<DadosEmissaoCertificadoCodafDto>> ObterDadosParaEmissaoCertificadosCodafAsync(long codafListaPresencaId);
        Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados);
        Task<IEnumerable<DadosProcessamentoCertificadoCodafDto>> ObterCertificadosParaProcessamentoAsync();
        Task AtualizarStatusProcessamentoAsync(long id, StatusProcessamentoCertificadoCodaf statusProcessamento, string? chaveObjetoArmazenamento, string? erroProcessamento);
        Task RecuperarCertificadosTravadosAsync();
    }
}
