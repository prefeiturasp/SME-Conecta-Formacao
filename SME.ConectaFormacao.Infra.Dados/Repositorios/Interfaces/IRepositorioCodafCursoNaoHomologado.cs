using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafCursoNaoHomologado : IRepositorioBaseAuditavel<CodafCursoNaoHomologado>
    {
        Task<ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>> ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(FiltroListagemResultadoCodafCursoNaoHomologadoDto filtro);
        Task<CodafCursoNaoHomologado?> ObterPorIdDetalhadoAsync(long id);
        Task ExcluirAsync(long id);
    }
}
