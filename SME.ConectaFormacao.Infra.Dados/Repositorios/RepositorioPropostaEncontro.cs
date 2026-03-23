using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioPropostaEncontro(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<PropostaEncontro>(contexto, conexao), IRepositorioPropostaEncontro
    {
        public async Task<PropostaEncontro?> ObterEncontroPorIdAsync(long encontroId)
        {
            const string query =
            """
            SELECT 
                id, proposta_id, hora_inicio, hora_fim, excluido, criado_em, criado_por, criado_login,
                alterado_em, alterado_por, alterado_login
            FROM proposta_encontro 
            WHERE id = @encontroId AND NOT excluido
            """;

            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaEncontro>(query, new { encontroId });
        }
        public async Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroIdAsync(params long[] encontroId)
        {
            const string query =
            """
            SELECT 
                id as Id, proposta_encontro_id as PropostaEncontroId, data_inicio as DataInicio, data_fim as DataFim,
                excluido as Excluido, criado_em as CriadoEm, criado_por as CriadoPor, criado_login as CriadoLogin,
                alterado_em as AlteradoEm, alterado_por as AlteradoPor, alterado_login as AlteradoLogin
            FROM proposta_encontro_data
            WHERE not excluido AND proposta_encontro_id = ANY(@encontroId)
            ORDER BY data_inicio;
            """;
            return await conexao.Obter().QueryAsync<PropostaEncontroData>(query, new { encontroId });
        }

        public async Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaTurmaAsync(long turmaId)
        {
            const string query =
            """
            SELECT            
                pe.id, pe.proposta_id, pe.hora_inicio, pe.hora_fim, pe.excluido, pe.criado_em, pe.criado_por,
                pe.criado_login, pe.alterado_em, pe.alterado_por, pe.alterado_login
            FROM proposta_encontro_turma pet
                 LEFT JOIN proposta_encontro pe ON pe.id = pet.proposta_encontro_id AND not pe.excluido
            WHERE pet.turma_id = @turmaId AND NOT pet.excluido;
                        
            SELECT 
                ped.id, ped.proposta_encontro_id, ped.data_inicio, ped.data_fim, ped.excluido, ped.criado_em,
                ped.criado_por, ped.criado_login, ped.alterado_em, ped.alterado_por, ped.alterado_login
            FROM proposta_encontro_turma pet
            LEFT JOIN proposta_encontro_data ped ON ped.proposta_encontro_id = pet.proposta_encontro_id AND NOT ped.excluido
            WHERE pet.turma_id = @turmaId AND NOT pet.excluido;
            """;

            var multiquery = await conexao.Obter().QueryMultipleAsync(query, new { turmaId });

            var encontros = await multiquery.ReadAsync<PropostaEncontro>();
            var datas = await multiquery.ReadAsync<PropostaEncontroData>();

            foreach (var encontro in encontros)
                encontro.Datas = datas.Where(t => t.PropostaEncontroId == encontro.Id);

            return encontros;
        }

        public async Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroIdAsync(params long[] encontroId)
        {
            const string query =
            """
            SELECT 
                pet.id as Id, pet.proposta_encontro_id as PropostaEncontroId, pet.turma_id as TurmaId,
                pet.excluido as Excluido, pet.criado_em as CriadoEm, pet.criado_por as CriadoPor, pet.criado_login as CriadoLogin,
                pet.alterado_em as AlteradoEm, pet.alterado_por as AlteradoPor, pet.alterado_login as AlteradoLogin,
            
                pt.id as Id, pt.proposta_id as PropostaId, pt.nome as Nome,
                pt.excluido as Excluido, pt.criado_em as CriadoEm, pt.criado_por as CriadoPor, pt.criado_login as CriadoLogin,
                pt.alterado_em as AlteradoEm, pt.alterado_por as AlteradoPor, pt.alterado_login as AlteradoLogin
            FROM proposta_encontro_turma pet
            INNER JOIN proposta_turma pt ON pt.id = pet.turma_id AND not pt.excluido
            WHERE not pet.excluido AND pet.proposta_encontro_id = ANY(@IdsEncontros)
            ORDER BY pt.nome;
            """;

            var lookup = new Dictionary<long, PropostaEncontroTurma>();
            await conexao.Obter().QueryAsync<PropostaEncontroTurma, PropostaTurma, PropostaEncontroTurma>(
                query,
                (pet, pt) =>
                {
                    if (!lookup.TryGetValue(pet.Id, out var propostaEncontroTurma))
                    {
                        propostaEncontroTurma = pet;
                        lookup.Add(propostaEncontroTurma.Id, propostaEncontroTurma);
                    }
                    propostaEncontroTurma.Turma = pt;
                    return propostaEncontroTurma;
                },
                new { IdsEncontros = encontroId },
                splitOn: "Id"
            );
            return lookup.Values;
        }
        public async Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaAsync(long propostaId)
        {
            const string query =
            """
            SELECT            
                 id, proposta_id as PropostaId, hora_inicio as HoraInicio, hora_fim as HoraFim, tipo, local,
                 excluido, criado_em as CriadoEm, criado_por as CriadoPor, criado_login as CriadoLogin,
                 alterado_em as AlteradoEm, alterado_por as AlteradoPor, alterado_login as AlteradoLogin
            FROM proposta_encontro 
            WHERE not excluido and proposta_id = @propostaId
            ORDER BY id
            """;
            return await conexao.Obter().QueryAsync<PropostaEncontro>(query, new { propostaId });
        }
        public async Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaAsync(long propostaId, int numeroPagina, int numeroRegistros)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var query = @"select 
                            id, 
                            proposta_id, 
                            hora_inicio,
                            hora_fim,
                            tipo,
                            local,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro 
                        where not excluido and proposta_id = @propostaId";

            query += " order by id";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return await conexao.Obter().QueryAsync<PropostaEncontro>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public async Task InserirEncontroAsync(long propostaId, PropostaEncontro encontro)
        {
            PreencherAuditoriaCriacao(encontro);

            encontro.PropostaId = propostaId;
            encontro.Id = (long)await conexao.Obter().InsertAsync(encontro);
        }

        public async Task InserirEncontroTurmasAsync(long propostaEncontroId, IEnumerable<PropostaEncontroTurma> turmas)
        {
            foreach (var turma in turmas)
            {
                PreencherAuditoriaCriacao(turma);

                turma.PropostaEncontroId = propostaEncontroId;
                turma.Id = (long)await conexao.Obter().InsertAsync(turma);
            }
        }

        public async Task InserirEncontroDatasAsync(long propostaEncontroId, IEnumerable<PropostaEncontroData> datas)
        {
            foreach (var data in datas)
            {
                PreencherAuditoriaCriacao(data);

                data.PropostaEncontroId = propostaEncontroId;
                data.Id = (long)await conexao.Obter().InsertAsync(data);
            }
        }

        public Task RemoverEncontrosAsync(IEnumerable<PropostaEncontro> encontros)
        {
            var encontro = encontros.First();
            PreencherAuditoriaAlteracao(encontro);

            var parametros = new
            {
                ids = encontros.Select(t => t.Id).ToArray(),
                encontro.AlteradoEm,
                encontro.AlteradoPor,
                encontro.AlteradoLogin
            };

            const string query =
            """
            UPDATE proposta_encontro_turma SET excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin WHERE NOT excluido AND proposta_encontro_id = any(@ids);
            UPDATE proposta_encontro_data SET excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin WHERE NOT excluido AND proposta_encontro_id = any(@ids);
            UPDATE proposta_encontro SET excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin WHERE NOT excluido AND id = any(@ids);
            """;

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarEncontroAsync(PropostaEncontro encontro)
        {
            PreencherAuditoriaAlteracao(encontro);
            await conexao.Obter().UpdateAsync(encontro);
        }

        public Task RemoverEncontroTurmasAsync(IEnumerable<PropostaEncontroTurma> turmas)
        {
            var turma = turmas.First();
            PreencherAuditoriaAlteracao(turma);

            var parametros = new
            {
                ids = turmas.Select(t => t.Id).ToArray(),
                turma.AlteradoEm,
                turma.AlteradoPor,
                turma.AlteradoLogin
            };

            const string query =
            """
            UPDATE proposta_encontro_turma
            SET excluido = true,
                alterado_em = @AlteradoEm,
                alterado_por = @AlteradoPor,
                alterado_login = @AlteradoLogin
            WHERE NOT excluido AND id = any(@ids)
            """;

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverEncontroDatasAsync(IEnumerable<PropostaEncontroData> datas)
        {
            var data = datas.First();
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                ids = datas.Select(t => t.Id).ToArray(),
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            const string query =
            """
            UPDATE proposta_encontro_data
            SET excluido = true,
                alterado_em = @AlteradoEm,
                alterado_por = @AlteradoPor,
                alterado_login = @AlteradoLogin
            WHERE NOT excluido AND id = any(@ids)
            """;

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarEncontroDataAsync(PropostaEncontroData data)
        {
            PreencherAuditoriaAlteracao(data);
            await conexao.Obter().UpdateAsync(data);
        }

        public async Task<int> ObterTotalEncontrosAsync(long propostaId)
        {
            const string query = 
                """
                SELECT COUNT(1) 
                FROM proposta_encontro 
                WHERE NOT excluido AND proposta_id = @propostaId
                """;
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<int> ObterQuantidadeDeTurmasComEncontroAsync(long propostaId)
        {
            const string query = 
                """                
                SELECT COUNT(DISTINCT pet.turma_id)
                FROM proposta_encontro_turma pet
                     INNER JOIN proposta_encontro pe ON pet.proposta_encontro_id = pe.id 
                WHERE NOT pet.excluido AND NOT pe.excluido AND pe.proposta_id = @propostaId
                """;
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }
    }
}
