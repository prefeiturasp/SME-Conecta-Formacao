using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioParametroSistema : RepositorioBaseAuditavel<ParametroSistema>, IRepositorioParametroSistema
    {
        public RepositorioParametroSistema(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<ParametroSistema> ObterParametroPorTipoEAno(TipoParametroSistema tipo, int ano = 0)
        {
            var query = @"select *
                            from parametro_sistema ps
                           where ano = @ano
                             and tipo = @tipo
                             and not excluido
                             and ativo";

            return await conexao.Obter().QueryFirstOrDefaultAsync<ParametroSistema>(query, new { tipo, ano });
        }

        public async Task<IEnumerable<string>> ObterDominiosPermitidosParaUesParceiras()
        {
            var tipo = TipoParametroSistema.DominioPermitidoCadastroUsuarioExterno;
            var query = @"select 
                        valor 
                        from parametro_sistema ps
                        where ativo = true
                        and tipo = @tipo ";

            return await conexao.Obter().QueryAsync<string>(query, new { tipo });
        }
    }
}
