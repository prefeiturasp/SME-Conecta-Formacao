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
        Task<int> ObterTotalRegistrosPorFiltros(long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao);
        Task<PropostaEncontro> ObterEncontroPorId(long encontroId);
        Task InserirEncontro(long propostaId, PropostaEncontro encontro);
        Task InserirEncontroTurmas(long propostaId, IEnumerable<PropostaEncontroTurma> turmas);
        Task InserirEncontroDatas(long propostaId, IEnumerable<PropostaEncontroData> datas);
        Task RemoverEncontros(IEnumerable<PropostaEncontro> encontros);
        Task AtualizarEncontro(PropostaEncontro encontro);
        Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroId(long encontroId);
        Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroId(long encontroId);
        Task RemoverEncontroTurmas(IEnumerable<PropostaEncontroTurma> turmas);
        Task RemoverEncontroDatas(IEnumerable<PropostaEncontroData> datas);
        Task AtualizarEncontroData(PropostaEncontroData data);
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long propostaId);
    }
}
