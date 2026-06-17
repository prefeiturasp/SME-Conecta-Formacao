using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Coordenadorias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCoordenadoria(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<Coordenadoria>(contexto, conexao), IRepositorioCoordenadoria
    {
        public async Task<Coordenadoria?> ObterComAreaPromotoraAsync(long id)
        {
            var coordenadoria = await ObterNaoExcluidosPorIdAsync(id);
            coordenadoria?.AreasPromotoras = await conexao.Obter().QueryAsync<AreaPromotora>("SELECT * FROM area_promotora WHERE coordenadoria_id = @id and not excluido", new { id });
            return coordenadoria;
        }

        public async Task<ResultadoPaginado<Coordenadoria>> ObterCoordenadoriaPaginadoAsync(string? nome, string? sigla, int pagina, int tamanhoPagina)
        {
            var condicoesWhere = new StringBuilder("WHERE NOT EXCLUIDO ");
            var parametros = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                condicoesWhere.Append("AND f_unaccent(NOME) ILIKE f_unaccent(@Nome) ");
                parametros.Add("Nome", $"%{nome}%");
            }

            if (!string.IsNullOrWhiteSpace(sigla))
            {
                condicoesWhere.Append("AND f_unaccent(SIGLA) ILIKE f_unaccent(@Sigla) ");
                parametros.Add("Sigla", $"%{sigla}%");
            }

            var conn = conexao.Obter();

            var sqlCount = new StringBuilder("SELECT COUNT(1) FROM coordenadoria ").Append(condicoesWhere);

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount.ToString(), parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<Coordenadoria>
                {
                    Itens = [],
                    PaginaAtual = pagina,
                    TamanhoPagina = tamanhoPagina,
                    TotalRegistros = 0
                };

            var sql = new StringBuilder("SELECT * FROM coordenadoria ").Append(condicoesWhere).Append("ORDER BY NOME OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY");

            parametros.Add("Offset", (pagina - 1) * tamanhoPagina);
            parametros.Add("TamanhoPagina", tamanhoPagina);

            var coordenadorias = await conn.QueryAsync<Coordenadoria>(sql.ToString(), parametros);

            return new ResultadoPaginado<Coordenadoria>
            {
                Itens = coordenadorias,
                PaginaAtual = pagina,
                TamanhoPagina = tamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public async Task<List<CoordenadoriaDto>> ObterCoordenadoriaSelectAsync()
        {
            var condicoesWhere = new StringBuilder("WHERE NOT EXCLUIDO ");

            var conn = conexao.Obter();

            var sql = new StringBuilder("SELECT * FROM coordenadoria ").Append(condicoesWhere).Append("ORDER BY NOME");

            var coordenadorias = await conn.QueryAsync<CoordenadoriaDto>(sql.ToString());

            return new List<CoordenadoriaDto>(coordenadorias);
        }
    }

}
