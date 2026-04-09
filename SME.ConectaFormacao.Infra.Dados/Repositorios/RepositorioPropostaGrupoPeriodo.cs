using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioPropostaGrupoPeriodo(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<PropostaGrupoPeriodo>(contexto, conexao), IRepositorioPropostaGrupoPeriodo
    {
        public override async Task<PropostaGrupoPeriodo> Atualizar(PropostaGrupoPeriodo entidade)
        {
            PreencherAuditoriaAlteracao(entidade);
            var connection = conexao.Obter();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sqlGrupo =
                """
                UPDATE proposta_grupo_periodo SET 
                    descricao = @Descricao, 
                    data_inicio = @DataInicio, 
                    data_fim = @DataFim, 
                    alterado_em = @AlteradoEm, 
                    alterado_por = @AlteradoPor, 
                    alterado_login = @AlteradoLogin,
                    excluido = @Excluido
                WHERE id = @Id;
                """;

                await connection.ExecuteAsync(sqlGrupo, entidade, transaction);

                if (entidade.TurmasVinculadas.Count != 0)
                {
                    await UpsertTurmasVinculadasAsync(connection, transaction, entidade);
                }
                transaction.Commit();
                return entidade;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async override Task<long> Inserir(PropostaGrupoPeriodo entidade)
        {
            PreencherAuditoriaCriacao(entidade);
            var connection = conexao.Obter();
            using var transaction = connection.BeginTransaction();

            try
            {
                var sqlGrupo =
                """
                INSERT INTO proposta_grupo_periodo 
                (proposta_id, descricao, data_inicio, data_fim, criado_em, criado_por, criado_login, excluido) 
                VALUES 
                (@PropostaId, @Descricao, @DataInicio, @DataFim, @CriadoEm, @CriadoPor, @CriadoLogin, false) 
                RETURNING id;
                """;

                var idGerado = await connection.ExecuteScalarAsync<long>(sqlGrupo, entidade, transaction);
                entidade.Id = idGerado;

                if (entidade.TurmasVinculadas.Count != 0)
                {
                    await UpsertTurmasVinculadasAsync(connection, transaction, entidade);
                }
                transaction.Commit();
                return idGerado;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override async Task<PropostaGrupoPeriodo> ObterPorId(long id)
        {
            var sql =
            """
            SELECT 
                gp.*, 
                t.* 
            FROM proposta_grupo_periodo gp
                 LEFT JOIN proposta_grupo_periodo_turma t ON gp.id = t.grupo_periodo_id
            WHERE gp.id = @Id AND NOT gp.excluido;
            """;

            PropostaGrupoPeriodo? grupoPeriodo = null;
            await conexao.Obter().QueryAsync<PropostaGrupoPeriodo, PropostaGrupoPeriodoTurma, PropostaGrupoPeriodo>(
                sql,
                (gp, turma) =>
                {
                    grupoPeriodo ??= gp;

                    if (turma != null)
                    {
                        grupoPeriodo.AdicionarTurma(turma.PropostaTurmaId);

                        if (turma.Excluido)
                        {
                            grupoPeriodo.TurmasVinculadas
                                .FirstOrDefault(t => t.PropostaTurmaId == turma.PropostaTurmaId)
                                ?.Excluido = true;
                        }
                    }
                    return grupoPeriodo;
                },
                new { Id = id },
                splitOn: "grupo_periodo_id");

#pragma warning disable CS8603 // Possible null reference return.
            return grupoPeriodo;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<PropostaGrupoPeriodo>> ObterPorPropostaIdAsync(long propostaId)
        {
            var sql =
            """
            SELECT 
                gp.*, 
                t.* 
            FROM proposta_grupo_periodo gp
                 LEFT JOIN proposta_grupo_periodo_turma t ON gp.id = t.grupo_periodo_id
            WHERE gp.proposta_id = @PropostaId AND NOT gp.excluido;
            """;
            var gruposPeriodo = new Dictionary<long, PropostaGrupoPeriodo>();
            await conexao.Obter().QueryAsync<PropostaGrupoPeriodo, PropostaGrupoPeriodoTurma, PropostaGrupoPeriodo>(
                sql,
                (gp, turma) =>
                {
                    if (!gruposPeriodo.TryGetValue(gp.Id, out var grupoPeriodo))
                    {
                        grupoPeriodo = gp;
                        gruposPeriodo.Add(grupoPeriodo.Id, grupoPeriodo);
                    }
                    if (turma != null)
                    {
                        grupoPeriodo.AdicionarTurma(turma.PropostaTurmaId);
                        if (turma.Excluido)
                        {
                            grupoPeriodo.TurmasVinculadas
                                .FirstOrDefault(t => t.PropostaTurmaId == turma.PropostaTurmaId)
                                ?.Excluido = true;
                        }
                    }
                    return grupoPeriodo;
                },
                new { PropostaId = propostaId },
                splitOn: "grupo_periodo_id");
            return gruposPeriodo.Values;
        }

        private async static Task UpsertTurmasVinculadasAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            PropostaGrupoPeriodo grupoPeriodo)
        {
            var sqlTurma =
            """
            INSERT INTO proposta_grupo_periodo_turma 
            (grupo_periodo_id, proposta_turma_id, criado_em, criado_por, criado_login, excluido) 
            VALUES 
            (@GrupoPeriodoId, @PropostaTurmaId, @CriadoEm, @CriadoPor, @CriadoLogin, @Excluido)
            ON CONFLICT (grupo_periodo_id, proposta_turma_id) 
            DO UPDATE SET 
                excluido = EXCLUDED.excluido,
                alterado_em = EXCLUDED.criado_em,
                alterado_por = EXCLUDED.criado_por,
                alterado_login = EXCLUDED.criado_login;
            """;

            var parametros = grupoPeriodo.TurmasVinculadas.Select(t => new
            {
                GrupoPeriodoId = grupoPeriodo.Id,
                t.PropostaTurmaId,
                CriadoEm = DateTimeExtension.HorarioBrasilia(),
                CriadoPor = grupoPeriodo.AlteradoPor ?? grupoPeriodo.CriadoPor,
                CriadoLogin = grupoPeriodo.AlteradoLogin ?? grupoPeriodo.CriadoLogin,
                t.Excluido
            });

            await connection.ExecuteAsync(sqlTurma, parametros, transaction);
        }
    }
}
