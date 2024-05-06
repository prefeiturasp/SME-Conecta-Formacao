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

        public Task<CargoFuncao> ObterCargoFuncaoOutros()
        {
            var query = "select id, nome, tipo, outros from cargo_funcao where outros and not excluido limit 1";
            return conexao.Obter().QueryFirstOrDefaultAsync<CargoFuncao>(query);
        }

        public async Task<IEnumerable<CargoFuncao>> ObterIgnorandoExcluidosPorTipo(CargoFuncaoTipo? tipo, bool exibirOutros)
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

            return await conexao.Obter().QueryAsync<CargoFuncao>(query, new { tipos });
        }

        public Task<IEnumerable<CargoFuncao>> ObterPorCodigoEol(long[] codigosCargosEol, long[] codigosFuncoesEol)
        {
            var query = @"select distinct cf.id, cf.nome, cf.tipo, cf.ordem 
                          from cargo_funcao cf 
                          inner join cargo_funcao_depara_eol cfd on cfd.cargo_funcao_id = cf.id and not cfd.excluido
                          where not cf.excluido";

            if (codigosCargosEol.Length > 0 && codigosFuncoesEol.Length > 0)
                query += " and (cfd.codigo_cargo_eol = any(@codigosCargosEol) or cfd.codigo_funcao_eol = any(@codigosFuncoesEol))";
            else if (codigosCargosEol.Length > 0 && codigosFuncoesEol.Length == 0)
                query += " and cfd.codigo_cargo_eol = any(@codigosCargosEol)";
            else if (codigosCargosEol.Length == 0 && codigosFuncoesEol.Length > 0)
                query += " and cfd.codigo_funcao_eol = any(@codigosFuncoesEol)";

            query += " order by cf.ordem";

            return conexao.Obter().QueryAsync<CargoFuncao>(query, new { codigosCargosEol, codigosFuncoesEol });
        }
    }
}
