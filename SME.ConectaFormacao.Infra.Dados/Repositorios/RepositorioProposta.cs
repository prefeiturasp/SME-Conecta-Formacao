using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioProposta : RepositorioBaseAuditavel<Proposta>, IRepositorioProposta
    {
        public RepositorioProposta(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<long> Inserir(IDbTransaction transacao, Proposta proposta)
        {
            PreencherAuditoriaCriacao(proposta);
            proposta.Id = (long)await conexao.Obter().InsertAsync(proposta, transacao);
            return proposta.Id;
        }

        public async Task InserirCriteriosValidacaoInscricao(IDbTransaction transacao, long id, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaCriacao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.PropostaId = id;
                criterioValidacaoInscricao.Id = (long)await conexao.Obter().InsertAsync(criterioValidacaoInscricao, transacao);
            }
        }

        public async Task InserirFuncoesEspecificas(IDbTransaction transacao, long id, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaCriacao(funcaoEspecifica);

                funcaoEspecifica.PropostaId = id;
                funcaoEspecifica.Id = (long)await conexao.Obter().InsertAsync(funcaoEspecifica, transacao);
            }
        }

        public async Task InserirPublicosAlvo(IDbTransaction transacao, long id, IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            foreach (var publicoAlvo in publicosAlvo)
            {
                PreencherAuditoriaCriacao(publicoAlvo);

                publicoAlvo.PropostaId = id;
                publicoAlvo.Id = (long)await conexao.Obter().InsertAsync(publicoAlvo, transacao);
            }
        }

        public async Task InserirvagasRemanecentes(IDbTransaction transacao, long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaCriacao(vagaRemanecente);

                vagaRemanecente.PropostaId = id;
                vagaRemanecente.Id = (long)await conexao.Obter().InsertAsync(vagaRemanecente, transacao);
            }
        }
    }
}
