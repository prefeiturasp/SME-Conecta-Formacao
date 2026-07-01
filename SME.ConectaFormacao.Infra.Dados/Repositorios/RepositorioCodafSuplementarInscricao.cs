using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioCodafSuplementarInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conectaFormacaoConexao)
        : RepositorioBaseAuditavel<CodafSuplementarInscricao>(contexto, conectaFormacaoConexao),
          IRepositorioCodafSuplementarInscricao
    {
        public async Task InserirVariosAsync(IEnumerable<CodafSuplementarInscricao> inscritosSuplementar)
        {
            foreach (var inscricao in inscritosSuplementar)
            {
                PreencherAuditoriaCriacao(inscricao);
                PreencherAuditoriaAlteracao(inscricao);
            }

            await conexao.Obter().ExecuteAsync(@"
                INSERT INTO PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO 
                (CODAF_SUPLEMENTAR_ID, INSCRICAO_ID, PERCENTUAL_FREQUENCIA, ATIVIDADE_OBRIGATORIO, CONCEITO_FINAL, APROVADO, criado_em, criado_por, alterado_em, alterado_por, criado_login, alterado_login, excluido) 
                VALUES 
                (@CodafSuplementarId, @InscricaoId, @PercentualFrequencia, @AtividadeObrigatorio, @ConceitoFinal, @Aprovado, @CriadoEm, @CriadoPor, @AlteradoEm, @AlteradoPor, @CriadoLogin, @AlteradoLogin, @Excluido);",
                inscritosSuplementar);

        }

        public async Task ExcluirPorCodafSuplementarIdAsync(long codafSuplementarId)
        {
            await conexao.Obter().ExecuteAsync(
                """
                DELETE FROM PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO 
                WHERE CODAF_SUPLEMENTAR_ID = @codafSuplementarId;

                SELECT SETVAL('public.codaf_suplementar_inscricao_id_seq', COALESCE((SELECT MAX(ID) FROM PUBLIC.CODAF_SUPLEMENTAR_INSCRICAO), 1));
                """, new { codafSuplementarId });
        }
    }
}