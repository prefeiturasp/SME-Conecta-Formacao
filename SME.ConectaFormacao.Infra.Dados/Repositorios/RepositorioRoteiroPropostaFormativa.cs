using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioRoteiroPropostaFormativa : RepositorioBaseAuditavel<RoteiroPropostaFormativa>, IRepositorioRoteiroPropostaFormativa
    {
        public RepositorioRoteiroPropostaFormativa(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<RoteiroPropostaFormativa> ObterUltimoRoteiroAtivo()
        {
            var query = @"select id, descricao from roteiro_proposta_formativa where not excluido order by id desc limit 1";
            return conexao.Obter().QueryFirstOrDefaultAsync<RoteiroPropostaFormativa>(query);
        }
    }
}
