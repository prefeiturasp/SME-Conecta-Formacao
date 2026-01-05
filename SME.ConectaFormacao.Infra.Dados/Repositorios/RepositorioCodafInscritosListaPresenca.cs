using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafInscritosListaPresenca(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao)
        : RepositorioBaseAuditavel<CodafInscricaoListaPresenca>(contexto, conectaFormacaoConexao),
          IRepositorioCodafInscritosListaPresenca
    {

        public async Task<ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>> ObterInscritosPorTurmaAsync(long propostaTurmaId, int numeroPagina, int numeroRegistros)
        {
            const string sqlBase = """
                FROM   PUBLIC.INSCRICAO AS I
                       INNER JOIN PUBLIC.USUARIO AS U  ON U.ID = I.USUARIO_ID 
                       LEFT JOIN PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA AS CI ON CI.INSCRICAO_ID = I.ID
                WHERE NOT U.EXCLUIDO
                AND NOT I.EXCLUIDO 
                AND I.SITUACAO = @situacao
                AND I.PROPOSTA_TURMA_ID = @propostaTurmaId
                """;
            const string sqlConsulta = $"""
                SELECT I.ID,
                       U.CPF,
                       U.NOME,
                       CI.PERCENTUAL_FREQUENCIA AS percentualFrequencia,
                       CI.ATIVIDADE_OBRIGATORIO AS atividadeObrigatorio,
                       CI.CONCEITO_FINAL AS conceitoFinal,
                       CI.APROVADO
                {sqlBase}
                ORDER BY U.NOME, U.CPF
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
            var inscritos = await conn.QueryAsync<ResultadoInscritoTurmaCodafListaPresencaDto>(sqlConsulta, parametros);
            return new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
            {
                Itens = inscritos,
                TotalRegistros = totalRegistros,
                PaginaAtual = numeroPagina,
                TamanhoPagina = numeroRegistros
            };

        }
        public async Task InserirVariosAsync(IEnumerable<CodafInscricaoListaPresenca> inscritosListaPresenca)
        {
            foreach (var inscricao in inscritosListaPresenca)
            {
                PreencherAuditoriaCriacao(inscricao);
                PreencherAuditoriaAlteracao(inscricao);
            }

            await conexao.Obter().ExecuteAsync(@"
                INSERT INTO PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA 
                (CODAF_LISTA_PRESENCA_ID, INSCRICAO_ID, PERCENTUAL_FREQUENCIA, ATIVIDADE_OBRIGATORIO, CONCEITO_FINAL, APROVADO, criado_em, criado_por, alterado_em, alterado_por, criado_login, alterado_login, excluido) 
                VALUES 
                (@CodafListaPresencaId, @InscricaoId, @PercentualFrequencia, @AtividadeObrigatorio, @ConceitoFinal, @Aprovado, @CriadoEm, @CriadoPor, @AlteradoEm, @AlteradoPor, @CriadoLogin, @AlteradoLogin, @Excluido);",
                inscritosListaPresenca);

        }

        public async Task ExcluirPorListaPresencaIdAsync(long codafListaPresencaId) =>
            await conexao.Obter().ExecuteAsync(@"
                DELETE FROM PUBLIC.CODAF_INSCRICAO_LISTA_PRESENCA 
                WHERE CODAF_LISTA_PRESENCA_ID = @codafListaPresencaId;",
                new { codafListaPresencaId });
    }
}