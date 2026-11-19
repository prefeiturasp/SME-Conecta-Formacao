using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafConferenciaInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao)
        : RepositorioBaseAuditavel<CodafConferenciaInscricao>(contexto, conectaFormacaoConexao),
          IRepositorioCodafConferenciaInscricao
    {
        public async Task<ResultadoPaginado<ResultadoInscritoTurmaCodafConferenciaDto>> ObterInscritosPorTurmaAsync(
            long propostaTurmaId,
            int numeroPagina,
            int numeroRegistros,
            long? codafConferenciaId = null)
        {
            const string sqlBase = """
                FROM   PUBLIC.INSCRICAO AS I
                       INNER JOIN PUBLIC.USUARIO AS U ON U.ID = I.USUARIO_ID 
                       LEFT JOIN PUBLIC.CODAF_CONFERENCIA_INSCRICAO AS CCI
                              ON CCI.INSCRICAO_ID = I.ID
                             AND CCI.CODAF_CONFERENCIA_ID = @codafConferenciaId
                             AND NOT CCI.EXCLUIDO
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
                       CCI.PARTICIPOU
                {sqlBase}
                ORDER BY COALESCE(U.NOME_SOCIAL, U.NOME), U.CPF
                LIMIT @limit OFFSET @offset;
                """;
            const string sqlCount = $"SELECT COUNT(1) {sqlBase}";
            var conn = conexao.Obter();
            var parametros = new DynamicParameters();
            parametros.Add("propostaTurmaId", propostaTurmaId);
            parametros.Add("situacao", SituacaoInscricao.Confirmada);
            parametros.Add("codafConferenciaId", codafConferenciaId, DbType.Int64);
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
            var inscritos = await conn.QueryAsync<ResultadoInscritoTurmaCodafConferenciaDto>(sqlConsulta, parametros);
            return new ResultadoPaginado<ResultadoInscritoTurmaCodafConferenciaDto>
            {
                Itens = inscritos,
                TotalRegistros = totalRegistros,
                PaginaAtual = numeroPagina,
                TamanhoPagina = numeroRegistros
            };
        }

        public async Task InserirVariosAsync(IEnumerable<CodafConferenciaInscricao> inscritos)
        {
            foreach (var inscricao in inscritos)
            {
                PreencherAuditoriaCriacao(inscricao);
                PreencherAuditoriaAlteracao(inscricao);
            }

            await conexao.Obter().ExecuteAsync(@"
                INSERT INTO PUBLIC.CODAF_CONFERENCIA_INSCRICAO 
                (codaf_conferencia_id, inscricao_id, participou, criado_em, criado_por, alterado_em, alterado_por, criado_login, alterado_login, excluido) 
                VALUES 
                (@CodafConferenciaId, @InscricaoId, @Participou, @CriadoEm, @CriadoPor, @AlteradoEm, @AlteradoPor, @CriadoLogin, @AlteradoLogin, @Excluido);",
                inscritos);
        }

        public async Task ExcluirPorCodafConferenciaIdAsync(long codafConferenciaId)
        {
            await conexao.Obter().ExecuteAsync(
                """
                DELETE FROM PUBLIC.CODAF_CONFERENCIA_INSCRICAO 
                WHERE codaf_conferencia_id = @codafConferenciaId;

                SELECT SETVAL('public.codaf_conferencia_inscricao_id_seq', COALESCE((SELECT MAX(ID) FROM PUBLIC.CODAF_CONFERENCIA_INSCRICAO), 1));
                """, new { codafConferenciaId });
        }
    }
}
