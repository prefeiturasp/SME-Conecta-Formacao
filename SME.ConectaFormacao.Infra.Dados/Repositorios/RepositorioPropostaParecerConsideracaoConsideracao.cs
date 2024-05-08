using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaParecerConsideracaoConsideracao : RepositorioBaseAuditavel<PropostaPareceristaConsideracao>, IRepositorioPropostaParecerConsideracao
    {
        public RepositorioPropostaParecerConsideracaoConsideracao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<IEnumerable<PropostaPareceristaConsideracao>> ObterPorPropostaIdECampo(long propostaId, CampoParecer campoParecer)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            campo,
                            descricao,
                            situacao,
                            usuario_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_parecerista_consideracao 
                        where proposta_id = @propostaId 
                              and not excluido
                              and campo = @campoParecer";
            
            return await conexao.Obter().QueryAsync<PropostaPareceristaConsideracao>(query, new { propostaId, campoParecer });
        }
    }
}