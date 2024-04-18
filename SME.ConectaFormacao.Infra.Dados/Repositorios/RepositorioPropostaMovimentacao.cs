using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaMovimentacao : RepositorioBaseAuditavel<PropostaMovimentacao>, IRepositorioPropostaMovimentacao
    {
        public RepositorioPropostaMovimentacao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<PropostaMovimentacao> ObterUltimoParecerPropostaId(long propostaId)
        {
            var query = @"select p.id as proposta_id,
                                 p.situacao, 
	                             pm.justificativa 
                          from proposta p 
                          left join proposta_movimentacao pm on pm.proposta_id = p.id and pm.situacao = p.situacao 
                          where p.id = @propostaId
                          order by pm.criado_em desc limit 1";
            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaMovimentacao>(query, new { propostaId });
        }

        public async Task<string> ObterUltimaJustificativaDevolucao(long propostaId)
        {
            const string query = @"select pm.justificativa 
                                    from proposta p 
                                    left join proposta_movimentacao pm on pm.proposta_id = p.id and pm.situacao = p.situacao 
                                    where p.id = @propostaId
                                    and p.situacao = @situacao
                                    and (pm.justificativa is not null or pm.justificativa <> '')
                                    order by pm.criado_em desc limit 1";
            
            return await conexao.Obter().QueryFirstOrDefaultAsync<string>(query, new { propostaId, situacao = (int)SituacaoProposta.Devolvida });
        }
    }
}