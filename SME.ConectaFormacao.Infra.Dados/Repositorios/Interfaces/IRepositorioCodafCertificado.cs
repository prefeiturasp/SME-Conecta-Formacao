using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafCertificado : IRepositorioBaseAuditavel<CodafCertificado>
    {
        Task<IEnumerable<DadosEmissaoCertificadoCodafDto>> ObterDadosParaEmissaoCertificadosCodafAsync(long codafListaPresencaId);
        Task<IEnumerable<DadosEmissaoCertificadoCodafDto>> ObterDadosParaEmissaoCertificadosCodafSuplementarAsync(long codafSuplementarId);
        Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados);
        Task<IEnumerable<DadosProcessamentoCodafDto>> ObterCertificadosParaProcessamentoAsync();
        Task AtualizarStatusProcessamentoAsync(long id, StatusProcessamentoCertificadoCodaf statusProcessamento, string? chaveObjetoArmazenamento, string? erroProcessamento);
        Task RecuperarCertificadosTravadosAsync();
        Task<ResultadoPaginado<MeusCertificadosCodafDto>> ObterMeusCertificadosPorFiltroAsync(FiltroMeusCertificadosCodafDto filtro);
        Task<DadosCertificadoUsuarioParaDownloadDto?> ObterCertificadoDisponivelDoUsuarioAsync(long codafCertificadoId);
        Task<ResultadoPaginado<ListagemCertificadosCodafDto>> ObterTodosCertificadosAsync(FiltroListagemTodosCertificadosCodafDto filtro);
        Task<IList<CodafCertificado>> ObterCertificadosDisponiveisPorListaDeIdAsync(List<long> certificadosId);
        Task AtualizaCodigoCertificado(long codafId, TipoCodaf tipoCodaf);
        Task InativarCertificadosAnterioresCursistaAsync(IEnumerable<long> idInscritos);
    }
}
