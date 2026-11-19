using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafConferenciaAnexo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafConferenciaAnexo>(contexto, conexao), IRepositorioCodafConferenciaAnexo
    {
        public async Task<IEnumerable<CodafConferenciaAnexo>> ObterPorCodafConferenciaIdAsync(long codafConferenciaId)
        {
            const string sql =
                """
                SELECT id, 
                       codaf_conferencia_id as CodafConferenciaId, 
                       arquivo_codigo as arquivoCodigo, 
                       nome_arquivo as nomeArquivo, 
                       extensao as extensao, 
                       tipo_anexo_id as tipoAnexoId, 
                       criado_em as criadoEm, 
                       criado_por as criadoPor, 
                       alterado_em as alteradoEm, 
                       alterado_por as alteradoPor, 
                       criado_login as criadoLogin, 
                       alterado_login as alteradoLogin, 
                       excluido
                FROM   public.codaf_conferencia_anexo
                WHERE  NOT EXCLUIDO
                  AND  codaf_conferencia_id = @codafConferenciaId
                """;

            var parametros = new { codafConferenciaId };
            var conn = conexao.Obter();
            return await conn.QueryAsync<CodafConferenciaAnexo>(sql, parametros);
        }
    }
}
