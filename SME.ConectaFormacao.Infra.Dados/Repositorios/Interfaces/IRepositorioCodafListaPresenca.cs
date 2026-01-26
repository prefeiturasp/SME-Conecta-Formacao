using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafListaPresenca : IRepositorioBaseAuditavel<CodafListaPresenca>
    {
        Task<bool> TurmaJaTemListaDePresencaAsync(long propostaTurmaId, long listaPresencaId = 0);
        Task<ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>> ObterListagemResultadoCodafListaPresencaPorFiltroAsync(FiltroListagemResultadoCodafListaPresencaDto filtro);
        Task<CodafListaPresenca?> ObterPorIdDetalhadoAsync(long id);
        Task<CodafListaPresenca?> ObterPorIdComPropostaEPropostaTurmaAsync(long id);
        Task ExcluirAsync(long id);
        Task<IEnumerable<DadosConsultaParaTxtEolDto>?> ObterDadosRemessaConclusaoCodafAsync(long id);
    }
}