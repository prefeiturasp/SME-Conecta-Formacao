using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCargoFuncao : RepositorioBaseAuditavel<CargoFuncao>, IRepositorioCargoFuncao
    {
        public RepositorioCargoFuncao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<bool> ExisteCargoFuncaoOutros(long[] ids)
        {
            var query = "select count(1) from cargo_funcao where id = any(@ids) and outros and not excluido limit 1";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { ids });
        }

        public Task<IEnumerable<CargoFuncao>> ObterIgnorandoExcluidosPorTipo(CargoFuncaoTipo? tipo, bool exibirOutros)
        {
            var tipos = new short[] {
                (short)CargoFuncaoTipo.Outros,
                (short)tipo.GetValueOrDefault()
            };

            var query = @"select id, nome, tipo, outros 
                          from cargo_funcao 
                          where not excluido";

            if (tipo.HasValue)
                query += " and tipo = any(@tipos)";

            if (!exibirOutros)
                query += " and not outros ";

            query += " order by ordem";

            return conexao.Obter().QueryAsync<CargoFuncao>(query, new { tipos });
        }
    }
}
