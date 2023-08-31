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

        public Task<IEnumerable<CargoFuncao>> ObterPorTipo(CargoFuncaoTipo? tipo)
        {
            var query = @"select id, nome, tipo from cargo_funcao ";

            if (tipo.HasValue)
                query += " where tipo = @tipo ";

            query += " order by nome";

            return conexao.Obter().QueryAsync<CargoFuncao>(query, new { tipo });
        }
    }
}
