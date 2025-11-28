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
    public class RepositorioSincronizador(IConectaFormacaoConexao conexao) : IRepositorioSincronizador
    {
        private readonly AsyncRetryPolicy _politicaRetry = Policy
            .Handle<NpgsqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(3, tentativa => TimeSpan.FromSeconds(Math.Pow(2, tentativa)));

        public async Task SincronizarLoteCargosEolAsync(List<CargoEol> cargos, string codigoDre)
        {
            const string deleteTableCommand = "DELETE FROM cargos_eol WHERE codigo_dre = @codigoDre";
            await _politicaRetry.ExecuteAsync(async () =>
            {
                var conn = (NpgsqlConnection)conexao.Obter(); 

                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();
                await using var transaction = await conn.BeginTransactionAsync();

                await conexao.Obter().ExecuteAsync(deleteTableCommand, new { codigoDre });
                await RealizarBulkInsertCargosAsync(conn, cargos);
                await transaction.CommitAsync();
            });            
        }
        private static async Task RealizarBulkInsertCargosAsync(NpgsqlConnection conn, List<CargoEol> dados)
        {
            const string copyCommand = @"
                COPY cargos_eol (
                    id, 
                    cd_cargo, 
                    cd_registro_funcional, 
                    codigo_dre, 
                    codigo_ue, 
                    sobreposto, 
                    data_atualizacao
                ) FROM STDIN (FORMAT BINARY)";

            using var writer = await conn.BeginBinaryImportAsync(copyCommand);

            foreach (var cargo in dados)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(cargo.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
                await writer.WriteAsync(cargo.CdCargo, NpgsqlTypes.NpgsqlDbType.Integer);
                await writer.WriteAsync(cargo.CdRegistroFuncional, NpgsqlTypes.NpgsqlDbType.Char); // Postgres Char
                await writer.WriteAsync(cargo.CodigoDre, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(cargo.CodigoUe, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(cargo.Sobreposto, NpgsqlTypes.NpgsqlDbType.Boolean);
                await writer.WriteAsync(cargo.DataAtualizacao, NpgsqlTypes.NpgsqlDbType.TimestampTz); // Timestamp com Timezone
            }

            // Completa a importação e envia ao banco
            await writer.CompleteAsync();
        }
    }
}
