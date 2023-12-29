using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioProposta : RepositorioBaseAuditavel<Proposta>, IRepositorioProposta
    {
        public RepositorioProposta(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task RemoverCriteriosValidacaoInscricao(IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            var criterioValidacaoInscricao = criteriosValidacaoInscricao.First();
            PreencherAuditoriaAlteracao(criterioValidacaoInscricao);

            var parametros = new
            {
                ids = criteriosValidacaoInscricao.Select(t => t.Id).ToArray(),
                criterioValidacaoInscricao.AlteradoEm,
                criterioValidacaoInscricao.AlteradoPor,
                criterioValidacaoInscricao.AlteradoLogin
            };

            var query = @"update proposta_criterio_validacao_inscricao 
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverVagasRemanecentes(IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            var vagaRemanecente = vagasRemanecentes.First();
            PreencherAuditoriaAlteracao(vagaRemanecente);

            var parametros = new
            {
                ids = vagasRemanecentes.Select(t => t.Id).ToArray(),
                vagaRemanecente.AlteradoEm,
                vagaRemanecente.AlteradoPor,
                vagaRemanecente.AlteradoLogin
            };

            var query = @"update proposta_vaga_remanecente 
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task InserirCriteriosValidacaoInscricao(long propostaId, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaCriacao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.PropostaId = propostaId;
                criterioValidacaoInscricao.Id = (long)await conexao.Obter().InsertAsync(criterioValidacaoInscricao);
            }
        }

        public async Task InserirFuncoesEspecificas(long propostaId, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaCriacao(funcaoEspecifica);

                funcaoEspecifica.PropostaId = propostaId;
                funcaoEspecifica.Id = (long)await conexao.Obter().InsertAsync(funcaoEspecifica);
            }
        }

        public async Task InserirPublicosAlvo(long propostaId, IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            foreach (var publicoAlvo in publicosAlvo)
            {
                PreencherAuditoriaCriacao(publicoAlvo);

                publicoAlvo.PropostaId = propostaId;
                publicoAlvo.Id = (long)await conexao.Obter().InsertAsync(publicoAlvo);
            }
        }

        public async Task InserirVagasRemanecentes(long propostaId, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaCriacao(vagaRemanecente);

                vagaRemanecente.PropostaId = propostaId;
                vagaRemanecente.Id = (long)await conexao.Obter().InsertAsync(vagaRemanecente);
            }
        }

        public Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            criterio_validacao_inscricao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_criterio_validacao_inscricao 
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaCriterioValidacaoInscricao>(query, new { propostaId });
        }

        public Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_funcao_especifica 
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaFuncaoEspecifica>(query, new { propostaId });
        }

        public Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_publico_alvo 
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaPublicoAlvo>(query, new { propostaId });
        }

        public Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            cargo_funcao_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_vaga_remanecente 
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaVagaRemanecente>(query, new { propostaId });
        }

        public Task RemoverFuncoesEspecificas(IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            var funcaoEspecifica = funcoesEspecificas.First();
            PreencherAuditoriaAlteracao(funcaoEspecifica);

            var parametros = new
            {
                ids = funcoesEspecificas.Select(t => t.Id).ToArray(),
                funcaoEspecifica.AlteradoEm,
                funcaoEspecifica.AlteradoPor,
                funcaoEspecifica.AlteradoLogin
            };

            var query = @"update proposta_funcao_especifica
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverPublicosAlvo(IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            var publicoAlvo = publicosAlvo.First();
            PreencherAuditoriaAlteracao(publicoAlvo);

            var parametros = new
            {
                ids = publicosAlvo.Select(t => t.Id).ToArray(),
                publicoAlvo.AlteradoEm,
                publicoAlvo.AlteradoPor,
                publicoAlvo.AlteradoLogin
            };

            var query = @"update proposta_publico_alvo
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        private static string MontarQueryPaginacao(long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, ref string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada)
        {
            var query = new StringBuilder();
            query.AppendLine("select p.*, ap.* ");
            query.AppendLine("from proposta p ");
            query.AppendLine("inner join area_promotora ap on ap.id = p.area_promotora_id and not ap.excluido");
            query.AppendLine("where not p.excluido ");

            if (propostaId.GetValueOrDefault() > 0)
                query.AppendLine(" and p.id = @propostaId");

            if (areaPromotoraId.GetValueOrDefault() > 0)
                query.AppendLine(" and p.area_promotora_id = @areaPromotoraId");

            if (formato.GetValueOrDefault() > 0)
                query.AppendLine(" and p.formato = @formato");

            if (publicoAlvoIds != null && publicoAlvoIds.Any())
                query.AppendLine(" and exists(select 1 from proposta_publico_alvo ppa where not ppa.excluido and ppa.proposta_id = p.id and ppa.cargo_funcao_id = any(@publicoAlvoIds) limit 1)");

            if (!string.IsNullOrEmpty(nomeFormacao))
            {
                nomeFormacao = "%" + nomeFormacao.ToLower() + "%";
                query.AppendLine(" and lower(p.nome_formacao) like @nomeFormacao");
            }

            if (periodoRealizacaoInicio.HasValue)
                query.AppendLine(" and data_realizacao_inicio::date >= @periodoRealizacaoInicio");

            if (periodoRealizacaoFim.HasValue)
                query.AppendLine(" and data_realizacao_fim::date <= @periodoRealizacaoFim");

            if (situacao.GetValueOrDefault() > 0)
                query.AppendLine(" and p.situacao = @situacao");

            if (formacaoHomologada.HasValue)
                query.AppendLine(" and p.formacao_homologada = @formacaoHomologada");

            return query.ToString();
        }

        public Task<int> ObterTotalRegistrosPorFiltros(long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada)
        {
            string query = string.Concat("select count(1) from (", MontarQueryPaginacao(propostaId, areaPromotoraId, formato, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao, formacaoHomologada), ") tb");
            return conexao.Obter().ExecuteScalarAsync<int>(query, new
            {
                propostaId,
                areaPromotoraId,
                formato,
                publicoAlvoIds,
                nomeFormacao,
                numeroHomologacao,
                periodoRealizacaoInicio = periodoRealizacaoInicio.GetValueOrDefault(),
                periodoRealizacaoFim = periodoRealizacaoFim.GetValueOrDefault(),
                situacao,
                formacaoHomologada
            });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryPaginacao(propostaId, areaPromotoraId, formato, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao, formacaoHomologada);

            query += " order by p.criado_em desc";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<Proposta, AreaPromotora, Proposta>(query, (proposta, areaPromotora) =>
                {
                    proposta.AreaPromotora = areaPromotora;
                    return proposta;
                },
                new
                {
                    propostaId,
                    numeroRegistros,
                    registrosIgnorados,
                    areaPromotoraId,
                    formato,
                    publicoAlvoIds,
                    nomeFormacao,
                    numeroHomologacao,
                    periodoRealizacaoInicio = periodoRealizacaoInicio.GetValueOrDefault(),
                    periodoRealizacaoFim = periodoRealizacaoFim.GetValueOrDefault(),
                    situacao,
                    formacaoHomologada
                },
                splitOn: "id, id");
        }

        public Task<PropostaEncontro> ObterEncontroPorId(long encontroId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            hora_inicio,
                            hora_fim,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro 
                        where id = @encontroId";
            return conexao.Obter().QueryFirstOrDefaultAsync<PropostaEncontro>(query, new { encontroId });
        }

        public Task<PropostaRegente> ObterPropostaRegentePorId(long id)
        {
            var query = @"SELECT
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
	                        nome_regente,
	                        mini_biografia,
	                        criado_em,
	                        criado_por,
	                        alterado_em,
	                        alterado_por,
	                        criado_login,
	                        alterado_login,
	                        excluido
                        FROM
	                        public.proposta_regente
	                        where not excluido and  id = @id;";
            return conexao.Obter().QueryFirstOrDefaultAsync<PropostaRegente>(query, new { id });
        }

        public Task<PropostaTutor> ObterPropostaTutorPorId(long id)
        {
            var query = @"select
	                            id,
	                            proposta_id,
	                            profissional_rede_municipal,
	                            registro_funcional,
	                            nome_tutor,
	                            criado_em,
	                            criado_por,
	                            alterado_em,
	                            alterado_por,
	                            criado_login,
	                            alterado_login,
	                            excluido
                            from
	                            public.proposta_tutor
	                        where not excluido and  id = @id;";
            return conexao.Obter().QueryFirstOrDefaultAsync<PropostaTutor>(query, new { id });
        }

        public async Task InserirEncontro(long propostaId, PropostaEncontro encontro)
        {
            PreencherAuditoriaCriacao(encontro);

            encontro.PropostaId = propostaId;
            encontro.Id = (long)await conexao.Obter().InsertAsync(encontro);
        }

        public async Task InserirEncontroTurmas(long propostaEncontroId, IEnumerable<PropostaEncontroTurma> turmas)
        {
            foreach (var turma in turmas)
            {
                PreencherAuditoriaCriacao(turma);

                turma.PropostaEncontroId = propostaEncontroId;
                turma.Id = (long)await conexao.Obter().InsertAsync(turma);
            }
        }

        public async Task InserirEncontroDatas(long propostaEncontroId, IEnumerable<PropostaEncontroData> datas)
        {
            foreach (var data in datas)
            {
                PreencherAuditoriaCriacao(data);

                data.PropostaEncontroId = propostaEncontroId;
                data.Id = (long)await conexao.Obter().InsertAsync(data);
            }
        }

        public Task RemoverEncontros(IEnumerable<PropostaEncontro> encontros)
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

            var query = @"
                    update proposta_encontro_turma set excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin where not excluido and proposta_encontro_id = any(@ids);
                    update proposta_encontro_data set excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin where not excluido and proposta_encontro_id = any(@ids);
                    update proposta_encontro set excluido = true, alterado_em = @AlteradoEm, alterado_por = @AlteradoPor, alterado_login = @AlteradoLogin where  not excluido and id = any(@ids);
                    ";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarEncontro(PropostaEncontro encontro)
        {
            PreencherAuditoriaAlteracao(encontro);
            await conexao.Obter().UpdateAsync(encontro);
        }

        public Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroId(params long[] encontroIds)
        {
            var query = @"select 
                            id, 
                            proposta_encontro_id, 
                            data_inicio,
                            data_fim,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro_data 
                        where proposta_encontro_id = any(@encontroIds) and not excluido";
            return conexao.Obter().QueryAsync<PropostaEncontroData>(query, new { encontroIds });
        }

        public async Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroId(params long[] encontroIds)
        {
            var query = @"select 
                            id, 
                            proposta_encontro_id, 
                            turma_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro_turma 
                        where proposta_encontro_id = any(@encontroIds) and not excluido;

                        SELECT 
                               pt.id,
                               pt.proposta_id,
                               pt.nome,
                               pt.excluido,   
                               pt.criado_em,
	                           pt.criado_por,
                               pt.criado_login,
                        	   pt.alterado_em,    
	                           pt.alterado_por,
	                           pt.alterado_login
                        FROM proposta_encontro_turma pet
                        INNER JOIN proposta_turma pt on pt.id = pet.turma_id and not pt.excluido
                        WHERE pet.proposta_encontro_id = any(@encontroIds) 
                          and not pet.excluido;";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { encontroIds });

            var encontroTurmas = queryMultiple.Read<PropostaEncontroTurma>();
            var turmas = queryMultiple.Read<PropostaTurma>();

            foreach (var encontroTurma in encontroTurmas)
                encontroTurma.Turma = turmas.FirstOrDefault(t => t.Id == encontroTurma.TurmaId);

            return encontroTurmas;
        }

        public Task RemoverEncontroTurmas(IEnumerable<PropostaEncontroTurma> turmas)
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

            var query = @"update proposta_encontro_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverEncontroDatas(IEnumerable<PropostaEncontroData> datas)
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

            var query = @"update proposta_encontro_data
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarEncontroData(PropostaEncontroData datas)
        {
            PreencherAuditoriaAlteracao(datas);
            await conexao.Obter().UpdateAsync(datas);
        }

        public Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            hora_inicio,
                            hora_fim,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro 
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaEncontro>(query, new { propostaId });
        }

        public Task<int> ObterTotalEncontros(long propostaId)
        {
            var query = @"select count(1) from proposta_encontro where not excluido and proposta_id = @propostaId";
            return conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public Task<int> ObterTotalRegentes(long propostaId)
        {
            var query = @"select
	                        count(distinct prt.turma_id)
                        from
	                        proposta_regente_turma prt
                        inner join proposta_regente pr on
	                        prt.proposta_regente_id = pr.id
                        where
	                        not prt.excluido
	                        and not pr.excluido
	                        and pr.proposta_id = @propostaId ";
            return conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public Task<int> ObterTotalTutores(long propostaId)
        {
            var query = @"select count(1) from proposta_tutor where not excluido and proposta_id = @propostaId";
            return conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public Task<IEnumerable<PropostaEncontro>> ObterEncontrosPaginados(int numeroPagina, int numeroRegistros, long propostaId)
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

            return conexao.Obter().QueryAsync<PropostaEncontro>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public Task<IEnumerable<PropostaRegente>> ObterRegentesPaginado(int numeroPagina, int numeroRegistros, long propostaId)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var query = @"select
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
	                        nome_regente,
	                        mini_biografia,
	                        criado_em,
	                        criado_por,
	                        alterado_em,
	                        alterado_por,
	                        criado_login,
	                        alterado_login,
	                        excluido
                        from
	                        public.proposta_regente
	                        where not excluido and proposta_id = @propostaId";

            query += " order by id";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<PropostaRegente>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public Task<IEnumerable<PropostaTutor>> ObterTutoresPaginado(int numeroPagina, int numeroRegistros, long propostaId)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var query = @"select
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
	                        nome_tutor,
	                        criado_em,
	                        criado_por,
	                        alterado_em,
	                        alterado_por,
	                        criado_login,
	                        alterado_login,
	                        excluido
                        from
	                        public.proposta_tutor
	                        where not excluido and proposta_id = @propostaId";

            query += " order by id";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<PropostaTutor>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public async Task<int> ObterQuantidadeDeTurmasComEncontro(long propostaId)
        {
            var query = @"select  count(distinct pet.turma_id)  
	                         from proposta_encontro_turma pet
	                         inner join proposta_encontro pe on pet.proposta_encontro_id = pe.id 
	                         where not pet.excluido and not pe.excluido and pe.proposta_id = @propostaId ";
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<IEnumerable<long>> ObterTurmasJaExistenteParaRegente(long propostaId, string? nomeRegente, string? registroFuncional, long[] turmaIds)
        {
            var query = new StringBuilder(@"select
	                                            distinct prt.turma_id
                                            from
	                                            proposta_regente pr
                                            inner join proposta_regente_turma prt on
	                                            pr.id = prt.proposta_regente_id
                                            where
	                                            not pr.excluido
	                                            and not prt.excluido 
	                                            and pr.proposta_id = @propostaId
	                                            and prt.turma_id = any(@turmaIds)");

            if (!string.IsNullOrEmpty(registroFuncional))
                query.AppendLine(" and pr.registro_funcional = @registroFuncional ");
            if (string.IsNullOrEmpty(registroFuncional) && !string.IsNullOrEmpty(nomeRegente))
                query.AppendLine(" and trim(pr.nome_regente) = @nomeRegente ");

            return await conexao.Obter().QueryAsync<long>(query.ToString(), new { propostaId, nomeRegente, registroFuncional, turmaIds });
        }

        public async Task<IEnumerable<long>> ObterTurmasJaExistenteParaTutor(long propostaId, string? nomeTutor, string? registroFuncional, long[] turmaIds)
        {
            var query = new StringBuilder(@"select
	                                            distinct ptt.turma_id
                                            from
	                                            proposta_tutor pt
                                            inner join proposta_tutor_turma ptt on
	                                            pt.id = ptt.proposta_tutor_id 
                                            where
	                                            not pt.excluido
	                                            and not ptt.excluido 
	                                            and pt.proposta_id = @propostaId
	                                            and ptt.turma_id = any(@turmaIds) ");

            if (!string.IsNullOrEmpty(registroFuncional))
                query.AppendLine(" and pt.registro_funcional = @registroFuncional ");
            if (string.IsNullOrEmpty(registroFuncional) && !string.IsNullOrEmpty(nomeTutor))
                query.AppendLine(" and trim(pt.nome_tutor) = @nomeTutor ");

            return await conexao.Obter().QueryAsync<long>(query.ToString(), new { propostaId, nomeTutor, registroFuncional, turmaIds });
        }

        public Task AtualizarSituacao(long id, SituacaoProposta situacaoProposta)
        {
            var query = @"update proposta 
                          set 
                            situacao = @situacaoProposta, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return conexao.Obter().ExecuteAsync(query, new
            {
                id,
                situacaoProposta,
                AlteradoEm = DateTimeExtension.HorarioBrasilia(),
                AlteradoPor = contexto.NomeUsuario,
                AlteradoLogin = contexto.UsuarioLogado
            });
        }

        public Task AtualizarSituacaoGrupoGestao(long id, SituacaoProposta situacaoProposta, long grupoGestaoId)
        {
            var query = @"update proposta 
                          set 
                            situacao = @situacaoProposta, 
                            grupo_gestao_id = @grupoGestaoId,
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return conexao.Obter().ExecuteAsync(query, new
            {
                id,
                situacaoProposta,
                grupoGestaoId,
                AlteradoEm = DateTimeExtension.HorarioBrasilia(),
                AlteradoPor = contexto.NomeUsuario,
                AlteradoLogin = contexto.UsuarioLogado
            });
        }

        public async Task InserirPalavraChave(long id, IEnumerable<PropostaPalavraChave> palavrasChaves)
        {
            foreach (var palavraChave in palavrasChaves)
            {
                PreencherAuditoriaCriacao(palavraChave);

                palavraChave.PropostaId = id;
                palavraChave.Id = (long)await conexao.Obter().InsertAsync(palavraChave);
            }
        }

        public async Task InserirModalidades(long id, IEnumerable<PropostaModalidade> modalidades)
        {
            foreach (var modalidade in modalidades)
            {
                PreencherAuditoriaCriacao(modalidade);

                modalidade.PropostaId = id;
                modalidade.Id = (long)await conexao.Obter().InsertAsync(modalidade);
            }
        }

        public async Task InserirAnosTurmas(long id, IEnumerable<PropostaAnoTurma> anosTurmas)
        {
            foreach (var anoTurma in anosTurmas)
            {
                PreencherAuditoriaCriacao(anoTurma);

                anoTurma.PropostaId = id;
                anoTurma.Id = (long)await conexao.Obter().InsertAsync(anoTurma);
            }
        }

        public async Task InserirComponentesCurriculares(long id, IEnumerable<PropostaComponenteCurricular> componentesCurriculares)
        {
            foreach (var componenteCurricular in componentesCurriculares)
            {
                PreencherAuditoriaCriacao(componenteCurricular);

                componenteCurricular.PropostaId = id;
                componenteCurricular.Id = (long)await conexao.Obter().InsertAsync(componenteCurricular);
            }
        }

        public async Task InserirCriterioCertificacao(long id, IEnumerable<PropostaCriterioCertificacao> criterios)
        {
            foreach (var criterio in criterios)
            {
                PreencherAuditoriaCriacao(criterio);

                criterio.PropostaId = id;
                criterio.Id = (long)await conexao.Obter().InsertAsync(criterio);
            }
        }

        public Task<IEnumerable<PropostaPalavraChave>> ObterPalavrasChavesPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            palavra_chave_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_palavra_chave 
                        where proposta_id = @id and not excluido ";
            return conexao.Obter().QueryAsync<PropostaPalavraChave>(query, new { id });
        }

        public Task<IEnumerable<PropostaModalidade>> ObterModalidadesPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            modalidade,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_modalidade 
                        where proposta_id = @id and not excluido ";
            return conexao.Obter().QueryAsync<PropostaModalidade>(query, new { id });
        }

        public Task<IEnumerable<PropostaAnoTurma>> ObterAnosTurmasPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            ano_turma_id AnoTurmaId,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_ano_turma 
                        where proposta_id = @id and not excluido ";
            return conexao.Obter().QueryAsync<PropostaAnoTurma>(query, new { id });
        }

        public Task<IEnumerable<PropostaComponenteCurricular>> ObterComponentesCurricularesPorId(long id)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            componente_curricular_id ComponenteCurricularId,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_componente_curricular 
                        where proposta_id = @id and not excluido ";
            return conexao.Obter().QueryAsync<PropostaComponenteCurricular>(query, new { id });
        }

        public Task<IEnumerable<PropostaCriterioCertificacao>> ObterCriterioCertificacaoPorPropostaId(long propostaId)
        {
            var query = @" select 
                            id, 
                            proposta_id, 
                            criterio_certificacao_id, 
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_criterio_certificacao pcc  
                        where proposta_id = @propostaId and not excluido ";
            return conexao.Obter().QueryAsync<PropostaCriterioCertificacao>(query, new { propostaId });
        }

        public Task RemoverPalavrasChaves(IEnumerable<PropostaPalavraChave> palavrasChaves)
        {
            var palavraChave = palavrasChaves.First();
            PreencherAuditoriaAlteracao(palavraChave);

            var parametros = new
            {
                ids = palavrasChaves.Select(t => t.Id).ToArray(),
                palavraChave.AlteradoEm,
                palavraChave.AlteradoPor,
                palavraChave.AlteradoLogin
            };

            var query = @"update proposta_palavra_chave
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverModalidades(IEnumerable<PropostaModalidade> modalidades)
        {
            var modalidade = modalidades.First();
            PreencherAuditoriaAlteracao(modalidade);

            var parametros = new
            {
                ids = modalidades.Select(t => t.Id).ToArray(),
                modalidade.AlteradoEm,
                modalidade.AlteradoPor,
                modalidade.AlteradoLogin
            };

            var query = @"update proposta_modalidade
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverAnosTurmas(IEnumerable<PropostaAnoTurma> anosTurmas)
        {
            var anoTurma = anosTurmas.First();
            PreencherAuditoriaAlteracao(anoTurma);

            var parametros = new
            {
                ids = anosTurmas.Select(t => t.Id).ToArray(),
                anoTurma.AlteradoEm,
                anoTurma.AlteradoPor,
                anoTurma.AlteradoLogin
            };

            var query = @"update proposta_ano_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverComponentesCurriculares(IEnumerable<PropostaComponenteCurricular> componenteCurriculares)
        {
            var componenteCurricular = componenteCurriculares.First();
            PreencherAuditoriaAlteracao(componenteCurricular);

            var parametros = new
            {
                ids = componenteCurriculares.Select(t => t.Id).ToArray(),
                componenteCurricular.AlteradoEm,
                componenteCurricular.AlteradoPor,
                componenteCurricular.AlteradoLogin
            };

            var query = @"update proposta_componente_curricular
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverCriterioCertificacao(IEnumerable<PropostaCriterioCertificacao> criterios)
        {
            var criterio = criterios.First();
            PreencherAuditoriaAlteracao(criterio);

            var parametros = new
            {
                ids = criterios.Select(t => t.Id).ToArray(),
                criterio.AlteradoEm,
                criterio.AlteradoPor,
                criterio.AlteradoLogin
            };

            var query = @"update proposta_criterio_certificacao
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task InserirPropostaRegente(long propostaId, PropostaRegente regente)
        {
            PreencherAuditoriaCriacao(regente);

            regente.PropostaId = propostaId;
            regente.NomeRegente = regente.NomeRegente.ToUpper();
            regente.Id = (long)await conexao.Obter().InsertAsync(regente);
        }

        public async Task InserirPropostaRegenteTurma(long propostaRegenteId, IEnumerable<PropostaRegenteTurma> regenteTurma)
        {
            foreach (var regente in regenteTurma)
            {
                PreencherAuditoriaCriacao(regente);

                regente.PropostaRegenteId = propostaRegenteId;
                regente.Id = (long)await conexao.Obter().InsertAsync(regente);
            }
        }

        public async Task<IEnumerable<PropostaRegenteTurma>> ObterRegenteTurmasPorRegenteId(params long[] regenteIds)
        {
            var query = @"SELECT
	                        id,
	                        proposta_regente_id,
	                        turma_id,
	                        criado_em,
	                        criado_por,
	                        alterado_em,
	                        alterado_por,
	                        criado_login,
	                        alterado_login,
	                        excluido
                        FROM proposta_regente_turma
	                    WHERE proposta_regente_id = any(@regenteIds) 
	                      and not excluido;

                        SELECT pt.id,
                               pt.proposta_id,
                               pt.nome,
                               pt.excluido,   
                               pt.criado_em,
	                           pt.criado_por,
                               pt.criado_login,
                        	   pt.alterado_em,    
	                           pt.alterado_por,
	                           pt.alterado_login
                        FROM proposta_regente_turma prt
                        INNER JOIN proposta_turma pt on pt.id = prt.turma_id and not pt.excluido
                        WHERE prt.proposta_regente_id = any(@regenteIds) 
                          and not prt.excluido;";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { regenteIds });

            var regenteTurmas = queryMultiple.Read<PropostaRegenteTurma>();
            var turmas = queryMultiple.Read<PropostaTurma>();

            foreach (var regenteTurma in regenteTurmas)
                regenteTurma.Turma = turmas.FirstOrDefault(t => t.Id == regenteTurma.TurmaId);

            return regenteTurmas;
        }

        public async Task InserirPropostaTutor(long propostaId, PropostaTutor tutor)
        {
            PreencherAuditoriaCriacao(tutor);
            tutor.NomeTutor = tutor.NomeTutor?.ToUpper();
            tutor.PropostaId = propostaId;
            tutor.Id = (long)await conexao.Obter().InsertAsync(tutor);
        }

        public async Task InserirPropostaTutorTurma(long propostaTutorId, IEnumerable<PropostaTutorTurma> tutorTurma)
        {
            foreach (var tutor in tutorTurma)
            {
                PreencherAuditoriaCriacao(tutor);

                tutor.PropostaTutorId = propostaTutorId;
                tutor.Id = (long)await conexao.Obter().InsertAsync(tutor);
            }
        }

        public Task ExcluirPropostasRegente(IEnumerable<PropostaRegente> propostaRegentes)
        {
            var data = propostaRegentes.First();
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                ids = propostaRegentes.Select(t => t.Id).ToArray(),
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_regente
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task ExcluirPropostaRegente(long regenteId)
        {
            var data = await ObterPropostaRegentePorId(regenteId);
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                id = regenteId,
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_regente
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id ";

            await conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task ExcluirPropostaTutor(long tutorId)
        {
            var data = await ObterPropostaTutorPorId(tutorId);
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                id = tutorId,
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_tutor
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id ";

            await conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task ExcluirPropostasTutor(IEnumerable<PropostaTutor> propostaTutors)
        {
            var data = propostaTutors.First();
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                ids = propostaTutors.Select(t => t.Id).ToArray(),
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_tutor
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task ExcluirPropostaTutorTurma(IEnumerable<PropostaTutorTurma> tutorTurmas)
        {
            var data = tutorTurmas.First();
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                ids = tutorTurmas.Select(t => t.Id).ToArray(),
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_tutor_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task ExcluirPropostaRegenteTurmas(IEnumerable<PropostaRegenteTurma> regenteTurmas)
        {
            var data = regenteTurmas.First();
            PreencherAuditoriaAlteracao(data);

            var parametros = new
            {
                ids = regenteTurmas.Select(t => t.Id).ToArray(),
                data.AlteradoEm,
                data.AlteradoPor,
                data.AlteradoLogin
            };

            var query = @"update proposta_regente_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarPropostaRegente(PropostaRegente propostaRegente)
        {
            PreencherAuditoriaAlteracao(propostaRegente);
            propostaRegente.NomeRegente = propostaRegente.NomeRegente.ToUpper();
            await conexao.Obter().UpdateAsync(propostaRegente);
        }

        public async Task AtualizarPropostaTutor(PropostaTutor propostaTutor)
        {
            PreencherAuditoriaAlteracao(propostaTutor);
            propostaTutor.NomeTutor = propostaTutor.NomeTutor?.ToUpper();
            await conexao.Obter().UpdateAsync(propostaTutor);
        }

        public async Task<IEnumerable<PropostaTutorTurma>> ObterTutorTurmasPorTutorId(params long[] tutorIds)
        {
            var query = @"SELECT
	                            id,
	                            proposta_tutor_id,
	                            turma_id,
	                            criado_em,
	                            criado_por,
	                            alterado_em,
	                            alterado_por,
	                            criado_login,
	                            alterado_login,
	                            excluido
                            FROM proposta_tutor_turma
	                        WHERE proposta_tutor_id = any(@tutorIds) 
	                          and not excluido;

                         SELECT 
                               pt.id,
                               pt.proposta_id,
                               pt.nome,
                               pt.excluido,   
                               pt.criado_em,
	                           pt.criado_por,
                               pt.criado_login,
                        	   pt.alterado_em,    
	                           pt.alterado_por,
	                           pt.alterado_login
                        FROM proposta_tutor_turma ptt
                        INNER JOIN proposta_turma pt on pt.id = ptt.turma_id and not pt.excluido
                        WHERE ptt.proposta_tutor_id = any(@tutorIds) 
                          and not ptt.excluido;";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { tutorIds });


            var tutorTurmas = queryMultiple.Read<PropostaTutorTurma>();
            var turmas = queryMultiple.Read<PropostaTurma>();

            foreach (var tutorTurma in tutorTurmas)
                tutorTurma.Turma = turmas.FirstOrDefault(t => t.Id == tutorTurma.TurmaId);

            return tutorTurmas;
        }

        public async Task InserirDres(long propostaId, IEnumerable<PropostaDre> propostaDres)
        {
            foreach (var propostaDre in propostaDres)
            {
                PreencherAuditoriaCriacao(propostaDre);

                propostaDre.PropostaId = propostaId;
                propostaDre.Id = (long)await conexao.Obter().InsertAsync(propostaDre);
            }
        }

        public Task RemoverDres(IEnumerable<PropostaDre> propostaDres)
        {
            var propostaDre = propostaDres.First();
            PreencherAuditoriaAlteracao(propostaDre);

            var parametros = new
            {
                ids = propostaDres.Select(t => t.Id).ToArray(),
                propostaDre.AlteradoEm,
                propostaDre.AlteradoPor,
                propostaDre.AlteradoLogin
            };

            var query = @"update proposta_dre
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task<IEnumerable<PropostaDre>> ObterDrePorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            dre_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_dre
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaDre>(query, new { propostaId });
        }

        public Task<IEnumerable<PropostaTurma>> ObterTurmasPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            nome,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_turma
                        where proposta_id = @propostaId and not excluido";
            return conexao.Obter().QueryAsync<PropostaTurma>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaTurmaDre>> ObterPropostaTurmasDresPorPropostaTurmaId(params long[] propostaTurmaIds)
        {
            var query = @"select 
                            id, 
                            proposta_turma_id, 
                            dre_id,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_turma_dre
                        where proposta_turma_id = any(@propostaTurmaIds) and not excluido; 
                        
                        select 
                            d.id,
                            d.nome
                        from proposta_turma_dre ptd
                        join dre d on d.id = ptd.dre_id and not d.excluido
                        where ptd.proposta_turma_id = any(@propostaTurmaIds) and not ptd.excluido;";

            var multiQuery = await conexao.Obter().QueryMultipleAsync(query, new { propostaTurmaIds });
            var turmaDres = multiQuery.Read<PropostaTurmaDre>();

            var dres = multiQuery.Read<Dre>();

            foreach (var turmaDre in turmaDres)
            {
                turmaDre.Dre = dres.FirstOrDefault(t => t.Id == turmaDre.DreId);
            }

            return turmaDres;
        }

        public async Task InserirTurmas(long propostaId, IEnumerable<PropostaTurma> propostaTurmas)
        {
            foreach (var propostaTurma in propostaTurmas)
            {
                PreencherAuditoriaCriacao(propostaTurma);

                propostaTurma.PropostaId = propostaId;
                propostaTurma.Id = (long)await conexao.Obter().InsertAsync(propostaTurma);
            }
        }

        public async Task InserirPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            foreach (var propostaTurmaDre in propostaTurmasDres)
            {
                PreencherAuditoriaCriacao(propostaTurmaDre);
                propostaTurmaDre.Id = (long)await conexao.Obter().InsertAsync(propostaTurmaDre);
            }
        }

        public Task RemoverTurmas(IEnumerable<PropostaTurma> propostaTurmas)
        {
            var propostaTurma = propostaTurmas.First();
            PreencherAuditoriaAlteracao(propostaTurma);

            var parametros = new
            {
                ids = propostaTurmas.Select(t => t.Id).ToArray(),
                propostaTurma.AlteradoEm,
                propostaTurma.AlteradoPor,
                propostaTurma.AlteradoLogin
            };

            var query = @"update proposta_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public Task RemoverPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            var propostaTurmaDre = propostaTurmasDres.First();
            PreencherAuditoriaAlteracao(propostaTurmaDre);

            var parametros = new
            {
                ids = propostaTurmasDres.Select(t => t.Id).ToArray(),
                propostaTurmaDre.AlteradoEm,
                propostaTurmaDre.AlteradoPor,
                propostaTurmaDre.AlteradoLogin
            };

            var query = @"update proposta_turma_dre
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task AtualizarTurmas(long propostaId, IEnumerable<PropostaTurma> propostaTurmas)
        {
            foreach (var propostaTurma in propostaTurmas)
            {
                PreencherAuditoriaAlteracao(propostaTurma);
                propostaTurma.Nome = propostaTurma.Nome.ToUpper();
                await conexao.Obter().UpdateAsync(propostaTurma);
            }
        }

        public async Task AtualizarPropostaTurmasDres(IEnumerable<PropostaTurmaDre> propostaTurmasDres)
        {
            foreach (var propostaTurmaDre in propostaTurmasDres)
            {
                PreencherAuditoriaAlteracao(propostaTurmaDre);
                propostaTurmaDre.DreId = propostaTurmaDre.DreId;
                await conexao.Obter().UpdateAsync(propostaTurmaDre);
            }
        }

        public Task<IEnumerable<long>> ObterListagemFormacoesPorFiltro(long[] publicosAlvosIds, string titulo, long[] areasPromotorasIds,
            DateTime? dataInicial, DateTime? dataFinal, int[] formatosIds, long[] palavrasChavesIds)
        {
            var tipoInscricao = TipoInscricao.Optativa;
            var situacao = SituacaoProposta.Publicada;
            titulo = titulo.NaoEhNulo() ? titulo.ToLower() : string.Empty;

            var query = @"select id 
                          from proposta p 
                          where not p.excluido 
                             and p.tipo_inscricao = @tipoInscricao 
                             and p.situacao = @situacao";

            if (areasPromotorasIds.PossuiElementos())
                query += " and p.area_promotora_id = any(@areasPromotorasIds) ";

            if (titulo.EstaPreenchido())
                query += " and f_unaccent(lower(p.nome_formacao)) LIKE ('%' || f_unaccent(@titulo) || '%') ";

            if (dataInicial.HasValue && dataFinal.HasValue)
                query += @" and (
                                (p.data_realizacao_inicio::date between @dataInicial and @dataFinal) or 
                                (p.data_realizacao_fim::date between @dataInicial and @dataFinal) or 
                                (p.data_realizacao_inicio::date <= @dataInicial and p.data_realizacao_fim::date >= @dataFinal)
                                )";

            if (formatosIds.PossuiElementos())
                query += " and p.formato = any(@formatosIds) ";

            if (publicosAlvosIds.PossuiElementos())
            {
                query += @" and exists(select 1 
                                       from proposta_publico_alvo ppa 
                                       where not ppa.excluido 
                                         and ppa.proposta_id = p.id 
                                         and ppa.cargo_funcao_id = any(@publicosAlvosIds)) ";
            }

            if (palavrasChavesIds.PossuiElementos())
            {
                query += @" and exists(select 1 
                                       from proposta_palavra_chave ppc 
                                       where not ppc.excluido 
                                         and ppc.proposta_id = p.id 
                                         and ppc.palavra_chave_id = any(@palavrasChavesIds)) ";
            }

            query += @" order by p.data_realizacao_inicio, p.data_realizacao_fim ";

            return conexao.Obter().QueryAsync<long>(query, new
            {
                palavrasChavesIds,
                formatosIds,
                dataInicial,
                dataFinal,
                areasPromotorasIds,
                titulo,
                publicosAlvosIds,
                tipoInscricao,
                situacao
            });
        }

        public async Task<IEnumerable<Proposta>> ObterPropostaResumidaPorId(long[] propostaIds)
        {
            var query = @"
                    select p.id, 
                        p.nome_formacao,
                        p.data_realizacao_inicio, 
                        p.data_realizacao_fim,
                        p.tipo_formacao,
                        p.formato formato,
                        p.data_inscricao_inicio,
                        p.data_inscricao_fim,
                        p.area_promotora_id,
                        p.arquivo_imagem_divulgacao_id
                    from public.proposta p  
                    where p.id = any(@propostaIds);

                    select ap.id,
	                       ap.nome 
                    from area_promotora ap 
                    where exists(select 1 from proposta p where ap.id = p.area_promotora_id and p.id = any(@propostaIds));

                    select a.id,
                           a.nome,
                           a.codigo
                    from arquivo a 
                    where exists(select 1 from proposta p where a.id = p.arquivo_imagem_divulgacao_id and p.id = any(@propostaIds));";

            var multiQuery = await conexao.Obter().QueryMultipleAsync(query, new { propostaIds });

            var propostas = multiQuery.Read<Proposta>();
            var areasPromotora = multiQuery.Read<AreaPromotora>();
            var arquivos = multiQuery.Read<Arquivo>();


            foreach (var proposta in propostas)
            {
                proposta.AreaPromotora = areasPromotora.FirstOrDefault(t => t.Id == proposta.AreaPromotoraId);
                proposta.ArquivoImagemDivulgacao = arquivos.FirstOrDefault(t => t.Id == proposta.ArquivoImagemDivulgacaoId);
            }

            return propostas;
        }

        public async Task<FormacaoDetalhada> ObterFormacaoDetalhadaPorId(long propostaId)
        {
            var tipoInscricao = TipoInscricao.Optativa;
            var situacao = SituacaoProposta.Publicada;

            var query = @"select
                            nome_formacao NomeFormacao,
                            tipo_formacao tipoFormacao,
                            formato,
                            data_realizacao_inicio dataRealizacaoInicio,
                            data_realizacao_fim dataRealizacaoFim,                            
                            data_inscricao_fim dataInscricaoFim,
                            justificativa
                        from proposta
                        where id = @propostaId 
                            and not excluido
                            and tipo_inscricao = @tipoInscricao 
                            and situacao = @situacao;

                          select
                              ap.nome
                        from proposta p 
                            join area_promotora ap on ap.id = p.area_promotora_id
                        where p.id = @propostaId 
                            and not p.excluido 
                            and not ap.excluido;

                        select 
                               cf.nome
                        from proposta_publico_alvo ppa
                        join cargo_funcao cf on cf.id = ppa.cargo_funcao_id
                        where ppa.proposta_id = @propostaId
                          and not ppa.excluido 
                          and not cf.excluido;

                        select 
                               pc.nome
                        from proposta_palavra_chave ppc
                        join palavra_chave pc on pc.id = ppc.palavra_chave_id
                        where ppc.proposta_id = @propostaId
                          and not ppc.excluido 
                          and not pc.excluido;

                        select 
                               pt.nome nome,
                               pe.local,
                               pe.hora_inicio horaInicio,
                               pe.hora_fim horaFim,
                               pet.proposta_encontro_id propostaEncontroId 
                        from proposta_turma pt
                        join proposta_encontro_turma pet on pet.turma_id = pt.id
                        join proposta_encontro pe on pe.id = pet.proposta_encontro_id 
                        where pt.proposta_id = @propostaId
                          and not pt.excluido
                          and not pet.excluido
                          and not pe.excluido
                        order by pt.nome, pe.hora_inicio;

                        select 
                              ped.data_inicio dataInicio,
                              ped.data_fim dataFim,
                              ped.proposta_encontro_id propostaEncontroId
                        from proposta_encontro pe 
                        join  proposta_encontro_data ped on ped.proposta_encontro_id = pe.id
                        where pe.proposta_id = @propostaId
                          and not pe.excluido
                          and not ped.excluido
                        order by ped.data_inicio;  

                        select a.nome,
                               a.codigo
                        from arquivo a 
                        where exists(select 1 from proposta p where a.id = p.arquivo_imagem_divulgacao_id and p.id = @propostaId);";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { propostaId, tipoInscricao, situacao });

            var formacaoDetalhe = queryMultiple.ReadFirst<FormacaoDetalhada>();
            formacaoDetalhe.AreaPromotora = queryMultiple.ReadFirst<string>();
            formacaoDetalhe.PublicosAlvo = queryMultiple.Read<string>();
            formacaoDetalhe.PalavrasChaves = queryMultiple.Read<string>();
            formacaoDetalhe.Turmas = queryMultiple.Read<FormacaoTurma>();
            var formacaoDatasTurmas = queryMultiple.Read<FormacaoTurmaData>();
            var arquivos = queryMultiple.Read<Arquivo>();
            formacaoDetalhe.ArquivoImagemDivulgacao = arquivos.Any() ? arquivos.FirstOrDefault() : null;

            foreach (var turma in formacaoDetalhe.Turmas)
                turma.Periodos = formacaoDatasTurmas.Where(w => w.PropostaEncontroId == turma.PropostaEncontroId);

            return formacaoDetalhe;
        }

        public Task InserirPropostaTurmaVagas(PropostaTurmaVaga propostaTurmaVaga)
        {
            PreencherAuditoriaCriacao(propostaTurmaVaga);
            return conexao.Obter().InsertAsync(propostaTurmaVaga);
        }

        public Task<PropostaTurma> ObterTurmaPorId(long propostaTurmaId)
        {
            return conexao.Obter().GetAsync<PropostaTurma>(propostaTurmaId);
        }

        public Task<IEnumerable<PropostaTurma>> ObterTurmasComVagaPorId(long propostaId)
        {
            var query = @"select 
                            pt.id, 
                            pt.proposta_id, 
                            pt.nome
                        from proposta_turma pt
                        where pt.proposta_id = @propostaId 
                          and not pt.excluido
                          and exists(select 1 
                                     from proposta_turma_vaga ptv 
                                     where ptv.proposta_turma_id = pt.id 
                                       and not ptv.excluido 
                                       and ptv.inscricao_id is null 
                                     limit 1)";

            return conexao.Obter().QueryAsync<PropostaTurma>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaTurmaId(long turmaId)
        {
            var query = @"select 
                            pe.id, 
                            pe.proposta_id, 
                            pe.hora_inicio,
                            pe.hora_fim,
                            pe.excluido,
                            pe.criado_em,
	                        pe.criado_por,
                            pe.criado_login,
                        	pe.alterado_em,    
	                        pe.alterado_por,
	                        pe.alterado_login
                        from proposta_encontro_turma pet
                        left join proposta_encontro pe on pe.id = pet.proposta_encontro_id and not pe.excluido
                        where pet.turma_id = @turmaId and not pet.excluido;
                        
                        select 
                            ped.id, 
                            ped.proposta_encontro_id, 
                            ped.data_inicio,
                            ped.data_fim,
                            ped.excluido,
                            ped.criado_em,
	                        ped.criado_por,
                            ped.criado_login,
                        	ped.alterado_em,    
	                        ped.alterado_por,
	                        ped.alterado_login
                        from proposta_encontro_turma pet
                        left join proposta_encontro_data ped on ped.proposta_encontro_id = pet.proposta_encontro_id and not ped.excluido
                        where pet.turma_id = @turmaId and not pet.excluido;";

            var multiquery = await conexao.Obter().QueryMultipleAsync(query, new { turmaId });

            var encontros = multiquery.Read<PropostaEncontro>();
            var datas = multiquery.Read<PropostaEncontroData>();

            foreach (var encontro in encontros)
                encontro.Datas = datas.Where(t => t.PropostaEncontroId == encontro.Id);

            return encontros;
        }
    }
}