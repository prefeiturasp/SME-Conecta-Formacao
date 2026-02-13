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
        Task InserirLoteAsync(IEnumerable<CodafCertificado> certificados);
        Task<IEnumerable<DadosProcessamentoCertificadoCodafDto>> ObterCertificadosParaProcessamentoAsync();
        Task AtualizarStatusProcessamentoAsync(long id, StatusProcessamentoCertificadoCodaf statusProcessamento, string? chaveObjetoArmazenamento, string? erroProcessamento);
        Task RecuperarCertificadosTravadosAsync();
        Task<ResultadoPaginado<ListagemResultadoCertificadoCodafUsuarioDto>> ObterListagemCertificadoDoUsuarioPorFiltroAsync(FiltroListagemResultadoCertificadoCodafUsuarioDto filtro);
        Task<DadosCertificadoUsuarioParaDownloadDto?> ObterCertificadoDisponivelDoUsuarioAsync(long codafCertificadoId);
        Task<ResultadoPaginado<ListagemResultadoCertificadoCodafAdminDto>> ObterListagemCertificadoPorFiltroAsync(FiltroListagemResultadoCertificadoCodafAdminDto filtro);
    }
}
