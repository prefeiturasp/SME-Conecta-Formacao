using Dapper;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioUe(IConectaFormacaoConexao conexao) : IRepositorioUe
    {
        public async Task<ResultadoPaginado<AutocompletarNomeUeDto>> AutocompletarNomeAsync(string termo, long dreId, int numeroPagina, int numeroRegistros)
        {
            var termoBusca = $"{termo}%";
            const string sqlBase = """
            FROM UE 
            WHERE f_unaccent(SIGLA_TIPO_ESCOLA || ' ' || NOME_ESCOLA) ILIKE f_unaccent(@TermoBusca) AND UE.DRE_ID = @DreId
            """;

            const string sql = $"""
            SELECT 
                id AS Id, 
                (SIGLA_TIPO_ESCOLA || ' ' || NOME_ESCOLA) AS Nome 
            {sqlBase}
            ORDER BY Nome
            LIMIT @limit OFFSET @offset;
            """;
            const string sqlCount = $"SELECT COUNT(1) {sqlBase}";
            var conn = conexao.Obter();
            var totalRegistros = await conn.ExecuteScalarAsync<int>(sqlCount, new { TermoBusca = termoBusca, DreId = dreId });
            if (totalRegistros == 0)
                return new()
                {
                    Itens = [],
                    TotalRegistros = 0,
                    PaginaAtual = numeroPagina,
                    TamanhoPagina = numeroRegistros
                };

            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var itens = await conn.QueryAsync<AutocompletarNomeUeDto>(sql, new { TermoBusca = termoBusca, DreId = dreId, limit = numeroRegistros, offset = registrosIgnorados });

            return new()
            {
                Itens = itens,
                TotalRegistros = totalRegistros,
                PaginaAtual = numeroPagina,
                TamanhoPagina = numeroRegistros
            };
        }
    }
}
