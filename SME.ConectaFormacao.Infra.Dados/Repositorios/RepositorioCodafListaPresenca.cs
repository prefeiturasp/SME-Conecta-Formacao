using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafListaPresenca(IConectaFormacaoConexao conexao, IContextoAplicacao contexto) :
        RepositorioBaseAuditavel<CodafListaPresenca>(contexto, conexao), IRepositorioCodafListaPresenca
    {
        public async Task<bool> TurmaJaTemListaDePresencaAsync(long propostaTurmaId, int listaPresencaId = 0)
        {
            const string query = """
                SELECT 1
                FROM CODAF_LISTA_PRESENCA
                WHERE PROPOSTA_TURMA_ID = @propostaTurmaId
                  AND ID <> @listaPresencaId
                """;

            var parametros = new
            {
                propostaTurmaId,
                listaPresencaId
            };

            return await conexao.Obter().QueryFirstOrDefaultAsync<bool>(query, parametros);
        }
    }
}
