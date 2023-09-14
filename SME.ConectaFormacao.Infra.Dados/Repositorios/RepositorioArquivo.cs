using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioArquivo : RepositorioBaseAuditavel<Arquivo>, IRepositorioArquivo
    {
        public RepositorioArquivo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<Arquivo> ObterPorCodigo(Guid codigo)
        {
            const string query = @"select * 
                                    from arquivo
                                    where codigo = @codigo";

            return conexao.Obter().QueryFirstOrDefaultAsync<Arquivo>(query, new { codigo });
        }

        public Task<IEnumerable<Arquivo>> ObterPorCodigos(Guid[] codigos)
        {
            const string query = @"select * 
                                    from arquivo
                                    where codigo = ANY(@codigos)";

            return conexao.Obter().QueryAsync<Arquivo>(query, new { codigos });
        }

        public Task<IEnumerable<Arquivo>> ObterPorIds(long[] ids)
        {
            const string query = @"select * 
                                    from arquivo
                                    where id = ANY(@ids)";

            return conexao.Obter().QueryAsync<Arquivo>(query, new { ids });
        }

        public Task<bool> ExcluirArquivoPorCodigo(Guid codigoArquivo)
        {
            var query = "delete from Arquivo where codigo = @codigoArquivo";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { codigoArquivo });
        }

        public Task<bool> ExcluirArquivoPorId(long id)
        {
            const string query = "delete from Arquivo where id = @id";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { id });
        }

        public Task<bool> ExcluirArquivosPorIds(long[] ids)
        {
            const string query = "delete from Arquivo where id = ANY(@ids)";
            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { ids });
        }

        public Task<long> ObterIdPorCodigo(Guid arquivoCodigo)
        {
            var query = @"select id
                            from arquivo 
                           where codigo = @arquivoCodigo";

            return conexao.Obter().QueryFirstOrDefaultAsync<long>(query, new { arquivoCodigo });
        }
    }
}
