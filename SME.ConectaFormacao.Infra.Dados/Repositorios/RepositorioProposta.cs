using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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

        private static string MontarQueryPaginacao(long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, ref string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            var query = new StringBuilder();
            query.AppendLine("select p.*, ap.* ");
            query.AppendLine("from proposta p ");
            query.AppendLine("left join area_promotora ap on ap.id = p.area_promotora_id and not ap.excluido");
            query.AppendLine("where not p.excluido ");

            if (propostaId.GetValueOrDefault() > 0)
                query.AppendLine(" and p.id = @propostaId");

            if (areaPromotoraId.GetValueOrDefault() > 0)
                query.AppendLine(" and p.area_promotora_id = @areaPromotoraId");

            if (modalidade.GetValueOrDefault() > 0)
                query.AppendLine(" and p.modalidade = @modalidade");

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

            return query.ToString();
        }

        public Task<int> ObterTotalRegistrosPorFiltros(long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            string query = string.Concat("select count(1) from (", MontarQueryPaginacao(propostaId, areaPromotoraId, modalidade, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao), ") tb");
            return conexao.Obter().ExecuteScalarAsync<int>(query, new
            {
                propostaId,
                areaPromotoraId,
                modalidade,
                publicoAlvoIds,
                nomeFormacao,
                numeroHomologacao,
                periodoRealizacaoInicio = periodoRealizacaoInicio.GetValueOrDefault(),
                periodoRealizacaoFim = periodoRealizacaoFim.GetValueOrDefault(),
                situacao
            });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? propostaId, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryPaginacao(propostaId, areaPromotoraId, modalidade, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao);

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
                    modalidade,
                    publicoAlvoIds,
                    nomeFormacao,
                    numeroHomologacao,
                    periodoRealizacaoInicio = periodoRealizacaoInicio.GetValueOrDefault(),
                    periodoRealizacaoFim = periodoRealizacaoFim.GetValueOrDefault(),
                    situacao
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

        public Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroId(params long[] encontroIds)
        {
            var query = @"select 
                            id, 
                            proposta_encontro_id, 
                            turma,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro_turma 
                        where proposta_encontro_id = any(@encontroIds) and not excluido";
            return conexao.Obter().QueryAsync<PropostaEncontroTurma>(query, new { encontroIds });
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
	                        count(distinct prt.turma)
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

        public Task EnviarPropostaParaDf(long propostaId)
        {
            var aguardandoDf = (int)SituacaoProposta.AguardandoAnaliseDf;
            var query = @"update 
                            proposta
                            set situacao  = @aguardandoDf
                          where id = @propostaId ";

            return conexao.Obter().ExecuteAsync(query, new { propostaId, aguardandoDf });
        }

        public async Task<int> ObterQuantidadeDeTurmasComEncontro(long propostaId)
        {
            var query = @"select  count(distinct pet.turma)  
	                         from proposta_encontro_turma pet
	                         inner join proposta_encontro pe on pet.proposta_encontro_id = pe.id 
	                         where pe.proposta_id = @propostaId ";
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<IEnumerable<int>> ObterTurmasJaExistenteParaRegente(long propostaId, string? nomeRegente, string? registroFuncional, int[] turmas)
        {
            var query = new StringBuilder(@"select
	                                            distinct prt.turma
                                            from
	                                            proposta_regente pr
                                            inner join proposta_regente_turma prt on
	                                            pr.id = prt.proposta_regente_id
                                            where
	                                            not pr.excluido
	                                            and not prt.excluido 
	                                            and pr.proposta_id = @propostaId
	                                            and prt.turma = any(@turmas)");

            if (!string.IsNullOrEmpty(registroFuncional))
                query.AppendLine(" and pr.registro_funcional = @registroFuncional ");
            if (string.IsNullOrEmpty(registroFuncional) && !string.IsNullOrEmpty(nomeRegente))
                query.AppendLine(" and trim(pr.nome_regente) = @nomeRegente ");

            return await conexao.Obter().QueryAsync<int>(query.ToString(), new { propostaId, nomeRegente, registroFuncional, turmas });
        }

        public async Task<IEnumerable<int>> ObterTurmasJaExistenteParaTutor(long propostaId, string? nomeTutor, string? registroFuncional, int[] turmas)
        {
            var query = new StringBuilder(@"select
	                                            distinct ptt.turma
                                            from
	                                            proposta_tutor pt
                                            inner join proposta_tutor_turma ptt on
	                                            pt.id = ptt.proposta_tutor_id 
                                            where
	                                            not pt.excluido
	                                            and not ptt.excluido 
	                                            and pt.proposta_id = @propostaId
	                                            and ptt.turma = any(@turmas) ");

            if (!string.IsNullOrEmpty(registroFuncional))
                query.AppendLine(" and pt.registro_funcional = @registroFuncional ");
            if (string.IsNullOrEmpty(registroFuncional) && !string.IsNullOrEmpty(nomeTutor))
                query.AppendLine(" and trim(pt.nome_tutor) = @nomeTutor ");

            return await conexao.Obter().QueryAsync<int>(query.ToString(), new { propostaId, nomeTutor, registroFuncional, turmas });
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

        public async Task InserirCriterioCertificacao(long id, IEnumerable<PropostaCriterioCertificacao> criterios)
        {
            foreach (var criterio in criterios)
            {
                PreencherAuditoriaCriacao(criterio);

                criterio.PropostaId = id;
                criterio.Id = (long)await conexao.Obter().InsertAsync(criterio);
            }
        }

        public Task<IEnumerable<PropostaPalavraChave>> ObterPalavraChavePorId(long id)
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

        public Task<IEnumerable<PropostaRegenteTurma>> ObterRegenteTurmasPorRegenteId(params long[] regenteIds)
        {
            var query = @"SELECT
	                        id,
	                        proposta_regente_id,
	                        turma,
	                        criado_em,
	                        criado_por,
	                        alterado_em,
	                        alterado_por,
	                        criado_login,
	                        alterado_login,
	                        excluido
                        FROM
	                        public.proposta_regente_turma
	                        where proposta_regente_id = any(@regenteIds) 
	                        and not excluido ";
            return conexao.Obter().QueryAsync<PropostaRegenteTurma>(query, new { regenteIds });
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

        public Task<IEnumerable<PropostaTutorTurma>> ObterTutorTurmasPorTutorId(params long[] tutorIds)
        {
            var query = @"select
	                            id,
	                            proposta_tutor_id,
	                            turma,
	                            criado_em,
	                            criado_por,
	                            alterado_em,
	                            alterado_por,
	                            criado_login,
	                            alterado_login,
	                            excluido
                            from
	                            public.proposta_tutor_turma
	                        where proposta_tutor_id = any(@tutorIds) 
	                        and not excluido ";
            return conexao.Obter().QueryAsync<PropostaTutorTurma>(query, new { tutorIds });
        }
    }
}