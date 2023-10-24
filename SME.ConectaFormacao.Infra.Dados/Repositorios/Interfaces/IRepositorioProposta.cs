using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioProposta : IRepositorioBaseAuditavel<Proposta>
    {
        Task RemoverCriteriosValidacaoInscricao(IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task RemoverVagasRemanecentes(IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task InserirCriteriosValidacaoInscricao(long propostaId, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task InserirFuncoesEspecificas(long propostaId, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task InserirPublicosAlvo(long propostaId, IEnumerable<PropostaPublicoAlvo> publicosAlvo);
        Task InserirVagasRemanecentes(long propostaId, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long propostaId);
        Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long propostaId);
        Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long propostaId);
        Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long propostaId);
        Task RemoverFuncoesEspecificas(IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task RemoverPublicosAlvo(IEnumerable<PropostaPublicoAlvo> publicoAlvo);
        Task<int> ObterTotalRegistrosPorFiltros(long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<PropostaEncontro> ObterEncontroPorId(long encontroId);
        Task InserirEncontro(long propostaId, PropostaEncontro encontro);
        Task InserirEncontroTurmas(long propostaId, IEnumerable<PropostaEncontroTurma> turmas);
        Task InserirEncontroDatas(long propostaId, IEnumerable<PropostaEncontroData> datas);
        Task RemoverEncontros(IEnumerable<PropostaEncontro> encontros);
        Task AtualizarEncontro(PropostaEncontro encontro);
        Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroId(params long[] encontroId);
        Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroId(params long[] encontroId);
        Task RemoverEncontroTurmas(IEnumerable<PropostaEncontroTurma> turmas);
        Task RemoverEncontroDatas(IEnumerable<PropostaEncontroData> datas);
        Task AtualizarEncontroData(PropostaEncontroData data);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long propostaId);
        Task<int> ObterTotalEncontros(long propostaId);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPaginados(int numeroPagina, int numeroRegistros, long propostaId);
        Task<IEnumerable<PropostaPalavraChave>> ObterPalavraChavePorId(long id);
        Task RemoverPalavrasChaves(IEnumerable<PropostaPalavraChave> palavrasChaves);
        Task InserirPalavraChave(long id, IEnumerable<PropostaPalavraChave> palavrasChaves);
        Task<IEnumerable<PropostaCriterioCertificacao>> ObterCriterioCertificacaoPorPropostaId(long propostaId);
        Task InserirCriterioCertificacao(long id, IEnumerable<PropostaCriterioCertificacao> criterios);
        Task RemoverCriterioCertificacao(IEnumerable<PropostaCriterioCertificacao> criterios);
        Task InserirPropostaRegente(long propostaId,PropostaRegente regente);
        Task InserirPropostaRegenteTurma(long propostaRegenteId,IEnumerable<PropostaRegenteTurma> regenteTurma);
        Task InserirPropostaTutor(long propostaId,PropostaTutor tutor);
        Task InserirPropostaTutorTurma(long propostaTutorId,IEnumerable<PropostaTutorTurma> tutorTurma);
        Task ExcluirPropostasRegente(IEnumerable<PropostaRegente> propostaRegentes);

        Task ExcluirPropostasTutor(IEnumerable<PropostaTutor> propostaTutors);
        Task ExcluirPropostaTutorTurma(IEnumerable<PropostaTutorTurma> tutorTurmas);
        Task ExcluirPropostaRegenteTurma(IEnumerable<PropostaRegenteTurma> regenteTurmas);
    }
}
