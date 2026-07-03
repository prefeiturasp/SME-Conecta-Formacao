using Npgsql;
using NpgsqlTypes;

namespace SME.ConectaFormacao.Infra.Dados.Extensoes
{
    public static class NpgsqlBinaryImporterExtensions
    {
        public static async Task EscreverNuloOuValorAsync<T>(this NpgsqlBinaryImporter writer, T? valor, NpgsqlDbType tipo) where T : struct
        {
            if (valor.HasValue)
                await writer.WriteAsync(valor.Value, tipo);
            else
                await writer.WriteNullAsync();
        }

        public static async Task EscreverNuloOuStringAsync(this NpgsqlBinaryImporter writer, string? valor, NpgsqlDbType tipo)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                await writer.WriteAsync(valor, tipo);
            else
                await writer.WriteNullAsync();
        }
    }
}