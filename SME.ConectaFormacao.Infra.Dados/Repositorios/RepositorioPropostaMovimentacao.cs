using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaMovimentacao : RepositorioBaseAuditavel<PropostaMovimentacao>, IRepositorioPropostaMovimentacao
    {
        public RepositorioPropostaMovimentacao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<PropostaMovimentacao> ObterUltimoParecerPropostaId(long propostaId)
        {
            var query = @"select p.id as proposta_id,
                                 p.situacao, 
	                             pm.justificativa 
                          from proposta p 
                          left join proposta_movimentacao pm on pm.proposta_id = p.id and pm.situacao = p.situacao 
                          where p.id = @propostaId
                          order by pm.criado_em desc limit 1";
            return conexao.Obter().QueryFirstOrDefaultAsync<PropostaMovimentacao>(query, new { propostaId });
        }
    }
}