using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafCursoNaoHomologadoInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao)
        : RepositorioBaseAuditavel<CodafCursoNaoHomologadoInscricao>(contexto, conectaFormacaoConexao),
          IRepositorioCodafCursoNaoHomologadoInscricao
    {

        public async Task<ResultadoPaginado<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>> ObterInscritosPorTurmaAsync(long propostaTurmaId, int numeroPagina, int numeroRegistros)
        {
            const string sqlBase = """
                FROM   PUBLIC.INSCRICAO AS I
                       INNER JOIN PUBLIC.USUARIO AS U  ON U.ID = I.USUARIO_ID 
                       LEFT JOIN PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO AS CI ON CI.INSCRICAO_ID = I.ID AND NOT CI.EXCLUIDO
                WHERE NOT U.EXCLUIDO
                AND NOT I.EXCLUIDO 
                AND I.SITUACAO = @situacao
                AND I.PROPOSTA_TURMA_ID = @propostaTurmaId
                """;
            const string sqlConsulta = $"""
                SELECT I.ID,
                       U.LOGIN,
                       U.CPF,
                       U.NOME,
                       U.NOME_SOCIAL,
                       CI.PARTICIPOU
                {sqlBase}
                ORDER BY COALESCE(U.NOME_SOCIAL, U.NOME), U.CPF
                LIMIT @limit OFFSET @offset;
                """;
            const string sqlCount = $"SELECT COUNT(1) {sqlBase}";
            var conn = conexao.Obter();
            var parametros = new DynamicParameters();
            parametros.Add("propostaTurmaId", propostaTurmaId);
            parametros.Add("situacao", SituacaoInscricao.Confirmada);
            var totalRegistros = await conn.ExecuteScalarAsync<int>(sqlCount, parametros);
            if (totalRegistros == 0)
                return new()
                {
                    Itens = [],
                    TotalRegistros = 0,
                    PaginaAtual = numeroPagina,
                    TamanhoPagina = numeroRegistros
                };

            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            parametros.Add("limit", numeroRegistros);
            parametros.Add("offset", registrosIgnorados);
            var inscritos = await conn.QueryAsync<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>(sqlConsulta, parametros);
            return new ResultadoPaginado<ResultadoInscritoTurmaCodafCursoNaoHomologadoDto>
            {
                Itens = inscritos,
                TotalRegistros = totalRegistros,
                PaginaAtual = numeroPagina,
                TamanhoPagina = numeroRegistros
            };

        }
        public async Task InserirVariosAsync(IEnumerable<CodafCursoNaoHomologadoInscricao> inscritosCursoNaoHomologado)
        {
            foreach (var inscricao in inscritosCursoNaoHomologado)
            {
                PreencherAuditoriaCriacao(inscricao);
                PreencherAuditoriaAlteracao(inscricao);
            }

            await conexao.Obter().ExecuteAsync(@"
                INSERT INTO PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO 
                (CODAF_LISTA_PRESENCA_ID, INSCRICAO_ID, PARTICIPOU, criado_em, criado_por, alterado_em, alterado_por, criado_login, alterado_login, excluido) 
                VALUES 
                (@CodafListaPresencaId, @InscricaoId, @Participou, @CriadoEm, @CriadoPor, @AlteradoEm, @AlteradoPor, @CriadoLogin, @AlteradoLogin, @Excluido);",
                inscritosCursoNaoHomologado);

        }

        public async Task ExcluirPorCursoNaoHomologadoIdAsync(long codafCursoNaoHomologadoId)
        {
            await conexao.Obter().ExecuteAsync(
                """
                DELETE FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO 
                WHERE CODAF_CURSO_NAO_HOMOLOGADO_ID = @codafCursoNaoHomologadoId;

                SELECT SETVAL('public.codaf_curso_nao_homologado_inscricao_id_seq', COALESCE((SELECT MAX(ID) FROM PUBLIC.CODAF_CURSO_NAO_HOMOLOGADO_INSCRICAO), 1));
                """, new { codafCursoNaoHomologadoId });
        }
    }
}