using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioDre : RepositorioBaseAuditavel<Dre>, IRepositorioDre
    {
        public RepositorioDre(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> VerificarSeDreExistePorCodigo(string codigoDre)
        {
            var query = @"select count(1) from dre d  where d.dre_id  = @codigoDre and not excluido";
            return await conexao.Obter().ExecuteScalarAsync<bool>(query, new {codigoDre});
        }

        public async Task<Dre> ObterDrePorCodigo(string codigoDre)
        {
            var query = @"select * from dre d  where d.dre_id  = @codigoDre";
            return await conexao.Obter().ExecuteScalarAsync<Dre>(query, new {codigoDre});
        }
    }
}