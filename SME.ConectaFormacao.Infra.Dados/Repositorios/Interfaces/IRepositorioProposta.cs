using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioProposta : IRepositorioBaseAuditavel<Proposta>
    {
        Task RemoverCriteriosValidacaoInscricao(IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task RemoverVagasRemanecentes(IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task InserirCriteriosValidacaoInscricao(long id, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task InserirFuncoesEspecificas(long id, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task InserirPublicosAlvo(long id, IEnumerable<PropostaPublicoAlvo> publicosAlvo);
        Task InserirVagasRemanecentes(long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long id);
        Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long id);
        Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long id);
        Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long id);
        Task RemoverFuncoesEspecificas(IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task RemoverPublicosAlvo(IEnumerable<PropostaPublicoAlvo> publicoAlvo);
        Task<int> ObterTotalRegistrosPorFiltros(long? id, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? id, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long id);
        Task InserirEncontros(long propostaId, IEnumerable<PropostaEncontro> encontrosInserir);
        Task InserirEncontroTurmas(long id, IEnumerable<PropostaEncontroTurma> turmas);
        Task InserirEncontroDatas(long id, IEnumerable<PropostaEncontroData> datas);
        Task RemoverEncontros(IEnumerable<PropostaEncontro> encontros);
        Task AtualizarEncontro(PropostaEncontro encontro);
        Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroIds(long[] encontroIds);
        Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroIds(long[] encontroIds);
        Task RemoverEncontroTurmas(IEnumerable<PropostaEncontroTurma> turmasExcluir);
        Task RemoverEncontroDatas(IEnumerable<PropostaEncontroData> datasExcluir);
        Task AtualizarEncontroData(PropostaEncontroData dataAlterar);
        Task<int> ObterTotalEncontros(long propostaId);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPaginados(int numeroPagina, int numeroRegistros, long propostaId);
    }
}
