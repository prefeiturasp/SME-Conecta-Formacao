using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCriterioValidacaoInscricao : RepositorioBaseAuditavel<CriterioValidacaoInscricao>, IRepositorioCriterioValidacaoInscricao
    {
        public RepositorioCriterioValidacaoInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<bool> ExisteCriterioValidacaoInscricaoOutros(long[] ids)
        {
            var query = "select count(1) from criterio_validacao_inscricao where id = any(@ids) and outros and not excluido limit 1";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { ids });
        }

        public Task<IEnumerable<CriterioValidacaoInscricao>> ObterIgnorandoExcluidos(bool exibirOutros)
        {
            var query = @"select id, nome, unico, outros, ordem 
                          from criterio_validacao_inscricao 
                          where not excluido";

            if (!exibirOutros)
                query += " and not outros ";

            query += " order by ordem";

            return conexao.Obter().QueryAsync<CriterioValidacaoInscricao>(query);
        }
    }
}
