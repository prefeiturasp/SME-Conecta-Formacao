using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaPareceristaConsideracao : RepositorioBaseAuditavel<PropostaPareceristaConsideracao>, IRepositorioPropostaPareceristaConsideracao
    {
        public RepositorioPropostaPareceristaConsideracao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<IEnumerable<PropostaPareceristaConsideracao>> ObterPorPropostaIdECampo(long propostaId, CampoConsideracao campoConsideracao)
        {
            var query = @"select 
                            ppc.id, 
                            ppc.proposta_parecerista_id, 
                            ppc.campo,
                            ppc.descricao,
                            ppc.excluido,
                            ppc.criado_em,
	                        ppc.criado_por,
                            ppc.criado_login,
                        	ppc.alterado_em,    
	                        ppc.alterado_por,
	                        ppc.alterado_login
                        from proposta_parecerista_consideracao ppc
                          join proposta_parecerista pp on pp.id = ppc.proposta_parecerista_id
                        where pp.proposta_id = @propostaId 
                              and not ppc.excluido
                              and not pp.excluido
                              and ppc.campo = @campoParecer";

            return await conexao.Obter().QueryAsync<PropostaPareceristaConsideracao>(query, new { propostaId, campoParecer = campoConsideracao });
        }

        public async Task<bool> ExistemConsideracoesPorParaceristaId(long pareceristaId)
        {
            var query = @"select count(1) 
                            from proposta_parecerista_consideracao 
                          where proposta_parecerista_id = @pareceristaId 
                            and not excluido limit 1";

            return await conexao.Obter().ExecuteScalarAsync<bool>(query, new { pareceristaId });
        }
    }
}