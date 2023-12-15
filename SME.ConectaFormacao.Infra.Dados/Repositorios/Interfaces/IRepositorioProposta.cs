﻿using SME.ConectaFormacao.Dominio.Entidades;
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
        Task<int> ObterTotalRegistrosPorFiltros(long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada);
        Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada);
        Task<PropostaEncontro> ObterEncontroPorId(long encontroId);
        Task InserirEncontro(long propostaId, PropostaEncontro encontro);
        Task InserirEncontroTurmas(long propostaId, IEnumerable<PropostaEncontroTurma> turmaIds);
        Task InserirEncontroDatas(long propostaId, IEnumerable<PropostaEncontroData> datas);
        Task RemoverEncontros(IEnumerable<PropostaEncontro> encontros);
        Task AtualizarEncontro(PropostaEncontro encontro);
        Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroId(params long[] encontroId);
        Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroId(params long[] encontroId);
        Task RemoverEncontroTurmas(IEnumerable<PropostaEncontroTurma> turmaIds);
        Task RemoverEncontroDatas(IEnumerable<PropostaEncontroData> datas);
        Task AtualizarEncontroData(PropostaEncontroData data);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long propostaId);
        Task<int> ObterTotalEncontros(long propostaId);
        Task<int> ObterTotalTutores(long propostaId);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPaginados(int numeroPagina, int numeroRegistros, long propostaId);
        Task<IEnumerable<PropostaRegente>> ObterRegentesPaginado(int numeroPagina, int numeroRegistros, long propostaId);
        Task<IEnumerable<PropostaPalavraChave>> ObterPalavrasChavesPorId(long id);
        Task RemoverPalavrasChaves(IEnumerable<PropostaPalavraChave> palavrasChaves);
        Task InserirPalavraChave(long id, IEnumerable<PropostaPalavraChave> palavrasChaves);
        Task InserirModalidades(long id, IEnumerable<PropostaModalidade> modalidades);
        Task RemoverModalidades(IEnumerable<PropostaModalidade> modalidades);
        Task InserirAnosTurmas(long id, IEnumerable<PropostaAnoTurma> anosTurmas);
        Task RemoverAnosTurmas(IEnumerable<PropostaAnoTurma> anosTurmas);
        Task InserirComponentesCurriculares(long id, IEnumerable<PropostaComponenteCurricular> componentesCurriculares);
        Task RemoverComponentesCurriculares(IEnumerable<PropostaComponenteCurricular> componenteCurriculares);
        Task<IEnumerable<PropostaCriterioCertificacao>> ObterCriterioCertificacaoPorPropostaId(long propostaId);
        Task InserirCriterioCertificacao(long id, IEnumerable<PropostaCriterioCertificacao> criterios);
        Task RemoverCriterioCertificacao(IEnumerable<PropostaCriterioCertificacao> criterios);
        Task InserirPropostaRegente(long propostaId, PropostaRegente regente);
        Task InserirPropostaRegenteTurma(long propostaRegenteId, IEnumerable<PropostaRegenteTurma> regenteTurma);
        Task<IEnumerable<PropostaRegenteTurma>> ObterRegenteTurmasPorRegenteId(params long[] regenteId);
        Task InserirPropostaTutor(long propostaId, PropostaTutor tutor);
        Task InserirPropostaTutorTurma(long propostaTutorId, IEnumerable<PropostaTutorTurma> tutorTurma);
        Task ExcluirPropostasRegente(IEnumerable<PropostaRegente> propostaRegentes);
        Task ExcluirPropostaRegente(long propostaRegenteId);
        Task ExcluirPropostasTutor(IEnumerable<PropostaTutor> propostaTutors);
        Task ExcluirPropostaTutorTurma(IEnumerable<PropostaTutorTurma> tutorTurmas);
        Task ExcluirPropostaRegenteTurmas(IEnumerable<PropostaRegenteTurma> regenteTurmas);
        Task<PropostaRegente> ObterPropostaRegentePorId(long id);
        Task<PropostaTutor> ObterPropostaTutorPorId(long id);
        Task AtualizarPropostaRegente(PropostaRegente propostaRegente);
        Task<int> ObterTotalRegentes(long propostaId);
        Task AtualizarPropostaTutor(PropostaTutor propostaTutor);
        Task<IEnumerable<PropostaTutorTurma>> ObterTutorTurmasPorTutorId(params long[] tutorIds);
        Task ExcluirPropostaTutor(long tutorId);
        Task<IEnumerable<PropostaTutor>> ObterTutoresPaginado(int numeroPagina, int numeroRegistros, long propostaId);
        Task<int> ObterQuantidadeDeTurmasComEncontro(long propostaId);
        Task<IEnumerable<long>> ObterTurmasJaExistenteParaRegente(long propostaId, string? nomeRegente, string? registroFuncional, long[] turmaIds);
        Task<IEnumerable<long>> ObterTurmasJaExistenteParaTutor(long propostaId, string? nomeTutor, string? registroFuncional, long[] turmaIds);
        Task AtualizarSituacao(long id, SituacaoProposta situacaoProposta);
        Task AtualizarSituacaoGrupoGestao(long id, SituacaoProposta situacaoProposta, long grupoGestaoId);
        Task InserirDres(long propostaId, IEnumerable<PropostaDre> propostaDres);
        Task RemoverDres(IEnumerable<PropostaDre> propostaDres);
        Task<IEnumerable<PropostaDre>> ObterDrePorId(long propostaId);
        Task<IEnumerable<PropostaTurma>> ObterTurmasPorId(long propostaId);
        Task InserirTurmas(long propostaId, IEnumerable<PropostaTurma> turmasInserir);
        Task RemoverTurmas(IEnumerable<PropostaTurma> turmasExcluir);
        Task AtualizarTurmas(long propostaId, IEnumerable<PropostaTurma> turmasInserir);
        Task<IEnumerable<PropostaModalidade>> ObterModalidadesPorId(long id);
        Task<IEnumerable<PropostaAnoTurma>> ObterAnosTurmasPorId(long id);
        Task<IEnumerable<PropostaComponenteCurricular>> ObterComponentesCurricularesPorId(long id);
        Task<IEnumerable<long>> ObterListagemFormacoesPorFiltro(long[] publicosAlvosIds, string titulo, long[] areasPromotorasIds, DateTime? dataInicial, DateTime? dataFinal, int[] formatosIds, long[] palavrasChavesIds);
        Task<IEnumerable<Proposta>> ObterPropostaResumidaPorId(long[] propostaIds);
        Task<IEnumerable<PropostaTurmaDre>> ObterPropostaTurmasDresPorPropostaTurmaId(long propostaTurmaId);
        Task InserirPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres);
        Task AtualizarPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres);
        Task RemoverPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres);
        Task<FormacaoDetalhada> ObterFormacaoDetalhadaPorId(long propostaId);
    }
}
