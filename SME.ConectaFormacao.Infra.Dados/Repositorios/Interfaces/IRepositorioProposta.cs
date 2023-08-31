﻿using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioProposta : IRepositorioBaseAuditavel<Proposta>
    {
        Task<bool> Atualizar(IDbTransaction transacao, Proposta proposta);
        Task RemoverCriteriosValidacaoInscricao(IDbTransaction transacao, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task RemoverVagasRemanecentes(IDbTransaction transacao, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task<long> Inserir(IDbTransaction transacao, Proposta proposta);
        Task InserirCriteriosValidacaoInscricao(IDbTransaction transacao, long id, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao);
        Task InserirFuncoesEspecificas(IDbTransaction transacao, long id, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task InserirPublicosAlvo(IDbTransaction transacao, long id, IEnumerable<PropostaPublicoAlvo> publicosAlvo);
        Task InserirVagasRemanecentes(IDbTransaction transacao, long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes);
        Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long id);
        Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long id);
        Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long id);
        Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long id);
        Task RemoverFuncoesEspecificas(IDbTransaction transacao, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas);
        Task RemoverPublicosAlvo(IDbTransaction transacao, IEnumerable<PropostaPublicoAlvo> publicoAlvo);
    }
}
