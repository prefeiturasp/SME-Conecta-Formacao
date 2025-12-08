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

        public async Task SincronizarLoteFuncaoAtividadeEolAsync(List<FuncaoAtividadeServidorEol> funcoesAtividade, string codigoDre)
        {
            const string deleteTableCommand = "DELETE FROM funcoes_atividades_eol WHERE codigo_dre = @codigoDre";
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
                    data_atualizacao,
                    data_posse,
                    nome_cargo,
                    tipo_vinculo
                ) FROM STDIN (FORMAT BINARY)";

            using var writer = await conn.BeginBinaryImportAsync(copyCommand);

            foreach (var cargo in dados)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(cargo.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
                await writer.WriteAsync(cargo.CodigoCargo, NpgsqlTypes.NpgsqlDbType.Integer);
                await writer.WriteAsync(cargo.CodigoRegistroFuncional, NpgsqlTypes.NpgsqlDbType.Char); // Postgres Char
                await writer.WriteAsync(cargo.CodigoDre, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(cargo.CodigoUe, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(cargo.Sobreposto, NpgsqlTypes.NpgsqlDbType.Boolean);
                await writer.WriteAsync(cargo.DataAtualizacao, NpgsqlTypes.NpgsqlDbType.TimestampTz); // Timestamp com Timezone
                if (cargo.DataPosse.HasValue)
                    await writer.WriteAsync(cargo.DataPosse, NpgsqlTypes.NpgsqlDbType.Date);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Date);
                if (!string.IsNullOrEmpty(cargo.NomeCargo))
                    await writer.WriteAsync(cargo.NomeCargo, NpgsqlTypes.NpgsqlDbType.Varchar);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                if (cargo.TipoVinculo.HasValue)
                    await writer.WriteAsync(cargo.TipoVinculo, NpgsqlTypes.NpgsqlDbType.Integer);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Integer);
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

        private static async Task RealizarBulkFuncaoAtividadeAsync(NpgsqlConnection conn, List<FuncaoAtividadeServidorEol> dados)
        {
            const string copyCommand = @"
                COPY funcoes_atividades_eol (
                    id, 
                    cd_registro_funcional, 
                    cd_tipo_funcao, 
                    codigo_dre,
                    codigo_ue, 
                    data_atualizacao,
                    data_posse,
                    nome_funcao,
                    tipo_vinculo
                ) FROM STDIN (FORMAT BINARY)";

            using var writer = await conn.BeginBinaryImportAsync(copyCommand);

            foreach (var funcaoAtividade in dados)
            {
                await writer.StartRowAsync();

                await writer.WriteAsync(funcaoAtividade.Id, NpgsqlTypes.NpgsqlDbType.Uuid);
                await writer.WriteAsync(funcaoAtividade.CdRegistroFuncional, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(funcaoAtividade.CdTipoFuncao, NpgsqlTypes.NpgsqlDbType.Integer);
                await writer.WriteAsync(funcaoAtividade.CdDre, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(funcaoAtividade.CdUe, NpgsqlTypes.NpgsqlDbType.Char);
                await writer.WriteAsync(funcaoAtividade.DataAtualizacao, NpgsqlTypes.NpgsqlDbType.TimestampTz);
                if (funcaoAtividade.DataPosse.HasValue)
                    await writer.WriteAsync(funcaoAtividade.DataPosse, NpgsqlTypes.NpgsqlDbType.Date);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Date);
                if (!string.IsNullOrEmpty(funcaoAtividade.NomeFuncao))
                    await writer.WriteAsync(funcaoAtividade.NomeFuncao, NpgsqlTypes.NpgsqlDbType.Varchar);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Varchar);
                if (funcaoAtividade.TipoVinculo.HasValue)
                    await writer.WriteAsync(funcaoAtividade.TipoVinculo, NpgsqlTypes.NpgsqlDbType.Integer);
                else
                    await writer.WriteAsync(DBNull.Value, NpgsqlTypes.NpgsqlDbType.Integer);
            }

            // Completa a importação e envia ao banco
            await writer.CompleteAsync();
        }
    }
}
