using Dapper;
using Npgsql;
using Polly;
using Polly.Retry;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioFuncaoAtividadeUsuario(IConectaFormacaoConexao conexao) : IRepositorioFuncaoAtividadeUsuario
    {
        private readonly AsyncRetryPolicy _politicaRetry = Policy
            .Handle<NpgsqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(3, tentativa => TimeSpan.FromSeconds(Math.Pow(2, tentativa)));

        public async Task<IEnumerable<FuncaoAtividadeServidorEol>> ObterPorRegistroFuncionalAsync(string cdRegistroFuncional)
        {
            const string query = @"SELECT id
                                        , cd_registro_funcional AS cdRegistroFuncional
                                        , cd_tipo_funcao AS cdTipoFuncao
                                        , codigo_ue AS cdUe
                                        , data_atualizacao AS dataAtualizacao
                                    FROM funcoes_atividades_eol
                                   WHERE cd_registro_funcional = @cdRegistroFuncional";

            var parametros = new DynamicParameters();
            parametros.Add("cdRegistroFuncional", cdRegistroFuncional);

            return await conexao.Obter().QueryAsync<FuncaoAtividadeServidorEol>(query, parametros);
        }

        public async Task<DateTime?> ObterDataUltimaAtualizacaoAsync(string codigoDre)
        {
            const string query = @"SELECT MAX(data_atualizacao)
                                    FROM funcoes_atividades_eol
                                   WHERE codigo_dre = @codigoDre";

            var parametros = new DynamicParameters();
            parametros.Add("codigoDre", codigoDre);

            return await _politicaRetry.ExecuteAsync(() => conexao.Obter().QueryFirstOrDefaultAsync<DateTime?>(query, parametros));
        }
    }
}
