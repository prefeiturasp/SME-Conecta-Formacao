using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaParecer : RepositorioBaseAuditavel<PropostaParecer>, IRepositorioPropostaParecer
    {
        public RepositorioPropostaParecer(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<IEnumerable<PropostaParecer>> ObterPorPropostaIdECampo(long propostaId, CampoParecer campoParecer)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            campo,
                            descricao,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_parecer 
                        where proposta_id = @propostaId 
                              and not excluido
                              and campo = @campoParecer";
            
            return await conexao.Obter().QueryAsync<PropostaParecer>(query, new { propostaId, campoParecer });
        }
    }
}