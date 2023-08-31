using Dapper;
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

        public async Task<bool> Atualizar(IDbTransaction transacao, Proposta proposta)
        {
            PreencherAuditoriaAlteracao(proposta);
            return await conexao.Obter().UpdateAsync(proposta, transacao);
        }

        public async Task RemoverCriteriosValidacaoInscricao(IDbTransaction transacao, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaAlteracao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.Excluido = true;
                await conexao.Obter().UpdateAsync(criterioValidacaoInscricao, transacao);
            }
        }

        public async Task RemoverVagasRemanecentes(IDbTransaction transacao, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaAlteracao(vagaRemanecente);
                vagaRemanecente.Excluido = true;
                await conexao.Obter().UpdateAsync(vagaRemanecente, transacao);
            }
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

        public async Task InserirVagasRemanecentes(IDbTransaction transacao, long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaCriacao(vagaRemanecente);

                vagaRemanecente.PropostaId = id;
                vagaRemanecente.Id = (long)await conexao.Obter().InsertAsync(vagaRemanecente, transacao);
            }
        }

        public Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            criterio_validacao_inscricao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_criterio_validacao_inscricao 
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaCriterioValidacaoInscricao>(query, new { id });
        }

        public Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_funcao_especifica 
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaFuncaoEspecifica>(query, new { id });
        }

        public Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_publico_alvo 
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaPublicoAlvo>(query, new { id });
        }

        public Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_vaga_remanecente 
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaVagaRemanecente>(query, new { id });
        }

        public async Task RemoverFuncoesEspecificas(IDbTransaction transacao, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaAlteracao(funcaoEspecifica);
                funcaoEspecifica.Excluido = true;
                await conexao.Obter().UpdateAsync(funcaoEspecifica, transacao);
            }
        }

        public async Task RemoverPublicosAlvo(IDbTransaction transacao, IEnumerable<PropostaPublicoAlvo> publicoAlvo)
        {
            foreach (var publico in publicoAlvo)
            {
                PreencherAuditoriaAlteracao(publico);
                publico.Excluido = true;
                await conexao.Obter().UpdateAsync(publico, transacao);
            }
        }
    }
}
