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

        public async Task SincronizarLoteFuncaoAtividadeEolAsync(List<FuncaoAtividadeUsuario> funcoesAtividade, string codigoDre)
        {
            const string deleteTableCommand = "DELETE FROM funcaoatividade_eol";
            await _politicaRetry.ExecuteAsync(async () =>
            {
                var conn = (NpgsqlConnection)conexao.Obter();

                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();
                await using var transaction = await conn.BeginTransactionAsync();

                await conexao.Obter().ExecuteAsync(deleteTableCommand, new { codigoDre });
                await RealizarBulkFuncaoAtividadeAsync(conn, funcoesAtividade);
                await transaction.CommitAsync();
            });
        }

        private static async Task RealizarBulkFuncaoAtividadeAsync(NpgsqlConnection conn, List<FuncaoAtividadeUsuario> dados)
        {
            const string copyCommand = @"
                COPY funcaoatividade_eol (
                    id, 
                    cd_registro_funcional, 
                    cd_tipo_funcao, 
                    codigo_ue, 
                    data_atualizacao
                ) FROM STDIN (FORMAT BINARY)";

            using var writer = await conn.BeginBinaryImportAsync(copyCommand);

            foreach (var funcaoAtividade in dados)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(funcaoAtividade.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
                await writer.WriteAsync(funcaoAtividade.CdRegistroFuncional, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(funcaoAtividade.CdTipoFuncao, NpgsqlTypes.NpgsqlDbType.Integer);
                await writer.WriteAsync(funcaoAtividade.CdUe, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(funcaoAtividade.DataAtualizacao, NpgsqlTypes.NpgsqlDbType.TimestampTz);
            }

            // Completa a importação e envia ao banco
            await writer.CompleteAsync();
        }

        public async Task LimparAtribuicaoServidorEolAsync(List<string> chavesExclusao)
        {
            const string deleteCommand = @"
                DELETE FROM atribuicoes_servidor_eol 
                WHERE chave_negocio = ANY(@chavesExclusao)";
            await _politicaRetry.ExecuteAsync(async () =>
            {
                await conexao.Obter().ExecuteAsync(deleteCommand, new { chavesExclusao });
            });
        }

        public async Task SincronizarLoteAtribuicaoServidorEolAsync(List<AtribuicaoServidorEol> atribuicaos)
        {
            await _politicaRetry.ExecuteAsync(async () =>
            {
                var conn = (NpgsqlConnection)conexao.Obter();
                if (conn.State != System.Data.ConnectionState.Open)
                    await conn.OpenAsync();
                await using var transaction = await conn.BeginTransactionAsync();
                await RealizarBulkInsertAtribuicoesServidoresAsync(conn, atribuicaos);
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
        private static async Task RealizarBulkInsertAtribuicoesServidoresAsync(NpgsqlConnection conn, List<AtribuicaoServidorEol> dados)
        {
            const string copyCommand = @"
                COPY atribuicoes_servidor_eol (
                    id,
                    chave_negocio,
                    cd_modalidade,
                    ano_serie,
                    cd_componente_curricular,
                    cd_registro_funcional,
                    codigo_ue,
                    data_atualizacao
                ) FROM STDIN (FORMAT BINARY)";

            using var writer = await conn.BeginBinaryImportAsync(copyCommand);

            foreach (var atribuicao in dados)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(atribuicao.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
                await writer.WriteAsync(atribuicao.ChaveNegocio, NpgsqlTypes.NpgsqlDbType.Varchar);
                await writer.WriteAsync((short)atribuicao.CdModalidade, NpgsqlTypes.NpgsqlDbType.Smallint);
                await writer.WriteAsync(atribuicao.AnoSerie, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(atribuicao.CdComponenteCurricular, NpgsqlTypes.NpgsqlDbType.Integer);
                await writer.WriteAsync(atribuicao.CdRegistroFuncional, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(atribuicao.CodigoUe, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(atribuicao.DataAtualizacao, NpgsqlTypes.NpgsqlDbType.TimestampTz); // Timestamp com Timezone
            }

            // Completa a importação e envia ao banco
            await writer.CompleteAsync();
        }
    }
}
