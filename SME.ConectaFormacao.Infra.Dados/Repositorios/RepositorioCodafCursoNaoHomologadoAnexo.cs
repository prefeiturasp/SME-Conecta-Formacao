using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafCursoNaoHomologadoAnexo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<CodafCursoNaoHomologadoAnexo>(contexto, conexao), IRepositorioCodafCursoNaoHomologadoAnexo
    {
        public async Task<IEnumerable<CodafCursoNaoHomologadoAnexo>> ObterPorCodafCursoNaoHomologadoIdAsync(long codafCursoNaoHomologadoId)
        {
            const string sql =
                """
                SELECT id, 
                       codaf_curso_nao_hom_id as CodafCursoNaoHomologadoId, 
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
                FROM   public.codaf_curso_nao_homologado_anexo
                WHERE  NOT EXCLUIDO
                  AND  codaf_curso_nao_hom_id = @codafCursoNaoHomologadoId
                """;

            var parametros = new { codafCursoNaoHomologadoId };
            var conn = conexao.Obter();
            return await conn.QueryAsync<CodafCursoNaoHomologadoAnexo>(sql, parametros);
        }
    }
}