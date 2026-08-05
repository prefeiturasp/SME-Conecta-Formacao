using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafSuplementar : IRepositorioBaseAuditavel<CodafSuplementar>
    {
        Task<ResultadoPaginado<ListagemResultadoCodafSuplementarDto>> ObterListagemResultadoCodafSuplementarPorFiltroAsync(FiltroListagemResultadoCodafSuplementarDto filtro);
        Task<CodafSuplementar?> ObterPorIdDetalhadoAsync(long id);
        Task ExcluirAsync(long id);
        Task<IEnumerable<DadosConsultaParaTxtEolDto>?> ObterDadosRemessaConclusaoCodafSuplementarAsync(long id);
        Task<CodafSuplementar?> ObterPorIdCodafListaPresenca(long idCodafListaPresenca);
    }
}