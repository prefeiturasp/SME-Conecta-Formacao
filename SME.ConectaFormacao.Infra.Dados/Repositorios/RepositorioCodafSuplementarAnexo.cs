using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafSuplementarAnexo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafSuplementarAnexo>(contexto, conexao), IRepositorioCodafSuplementarAnexo
    {
        public async Task<IEnumerable<CodafSuplementarAnexo>> ObterPorCodafSuplementarIdAsync(long codafSuplementarId)
        {
            const string sql =
                """
                SELECT id, 
                       codaf_suplementar_id as codafSuplementarId, 
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
                FROM   public.codaf_anexo
                WHERE  NOT EXCLUIDO
                  AND  codaf_suplementar_id = @codafSuplementarId
                """;

            var parametros = new { codafSuplementarId };
            var conn = conexao.Obter();
            return await conn.QueryAsync<CodafSuplementarAnexo>(sql, parametros);
        }
    }
}