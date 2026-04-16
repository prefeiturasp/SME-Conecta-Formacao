using Dapper;
using Dommel;
using Minio.DataModel.Notification;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioProposta(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<Proposta>(contexto, conexao), IRepositorioProposta
    {
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

        public async Task<PropostaTurma> ObterTurmaDaPropostaComDresPorId(long propostaTurmaId)
        {
            var propostaTurma = await conexao.Obter().GetAsync<PropostaTurma>(propostaTurmaId);

            if (propostaTurma != null)
            {
                var sql = @" SELECT d.id AS Id, d.dre_id AS CodigoDre, d.abreviacao AS DreAbreviacao, d.nome AS DreNome
                  FROM proposta_turma_dre ptd
                  INNER JOIN dre d ON d.id = ptd.dre_id
                  WHERE ptd.proposta_turma_id = @Id 
                  AND ptd.excluido IS NOT TRUE
                  AND d.excluido IS NOT TRUE ";

                var dres = await conexao.Obter().QueryAsync<PropostaTurmaDre>(sql, new { Id = propostaTurmaId });

                propostaTurma.Dres = dres.ToList();
            }

            return propostaTurma;
        }

        public async Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaCriterioValidacaoInscricao>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaFuncaoEspecifica>(query, new { propostaId });
        }

        public async Task<bool> ExisteCargoFuncaoOutrosNaProposta(long propostaId)
        {
            var tipoOutros = (int)CargoFuncaoTipo.Outros;
            var query = @"select count(*)>0 as existe
				                from proposta_funcao_especifica pfe 
				                inner join cargo_funcao cf on cf.id = pfe.cargo_funcao_id 
			                    where not pfe.excluido 
			                    and pfe.proposta_id  = @propostaId 
			                     and cf.tipo = @tipoOutros ";
            return await conexao.Obter().QueryFirstOrDefaultAsync<bool>(query, new { propostaId, tipoOutros });
        }

        public async Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaPublicoAlvo>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaVagaRemanecente>(query, new { propostaId });
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

        private static string MontarQueryPaginacao(long? areaPromotoraIdUsuarioLogado, long? propostaId, long? areaPromotoraId, Formato? formato, long[] publicoAlvoIds, ref string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada, string loginUsuarioLogado, Guid perfilUsuarioLogado)
        {
            var query = new StringBuilder();
            query.AppendLine("select p.id, p.tipo_formacao, p.formato, p.nome_formacao, p.data_realizacao_inicio, p.data_realizacao_fim, p.situacao, p.formacao_homologada, p.numero_homologacao, p.revalidacao, p.justificativa_revalidacao, ap.id, ap.nome ");
            query.AppendLine("from proposta p ");
            query.AppendLine("inner join area_promotora ap on ap.id = p.area_promotora_id and not ap.excluido");
            query.AppendLine("where not p.excluido ");

            if (areaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
                query.AppendLine(" and p.area_promotora_id = @areaPromotoraIdUsuarioLogado");

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

            if (numeroHomologacao.HasValue)
                query.AppendLine(" and p.numero_homologacao = @numeroHomologacao");

            if (perfilUsuarioLogado.EhPerfilParecerista())
                query.AppendLine(@" and p.situacao = @situacaoAguardandoParecerista 
                                    and p.id in (select proposta_id 
                                                 from proposta_parecerista 
                                                 where not excluido 
                                                   and registro_funcional = @loginUsuarioLogado)");

            return query.ToString();
        }

        public async Task<ResultadoPaginado<Proposta>> ObterPropostaPorFiltroAsync(FiltroListagemPropostaDto filtro)
        {
            const string sqlBaseJoins = """
                FROM proposta p
                INNER JOIN area_promotora ap ON ap.id = p.area_promotora_id AND NOT ap.excluido
                """;
            const string sqlBaseOrderBy = " ORDER BY p.criado_em DESC ";

            var condicoesWhere = new StringBuilder(" WHERE NOT p.excluido ");
            var parametros = new DynamicParameters();

            if (filtro.AreaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
            {
                condicoesWhere.AppendLine(" and p.area_promotora_id = @areaPromotoraIdUsuarioLogado");
                parametros.Add("areaPromotoraIdUsuarioLogado", filtro.AreaPromotoraIdUsuarioLogado);
            }

            if (filtro.PropostaId.GetValueOrDefault() > 0)
            {
                condicoesWhere.AppendLine(" and p.id = @propostaId");
                parametros.Add("propostaId", filtro.PropostaId);
            }

            if (filtro.AreaPromotoraId.GetValueOrDefault() > 0)
            {
                condicoesWhere.AppendLine(" and p.area_promotora_id = @areaPromotoraId");
                parametros.Add("areaPromotoraId", filtro.AreaPromotoraId);
            }

            if (filtro.Formato.GetValueOrDefault() > 0)
            {
                condicoesWhere.AppendLine(" and p.formato = @formato");
                parametros.Add("formato", filtro.Formato);
            }

            if (filtro.PublicoAlvoIds != null && filtro.PublicoAlvoIds.Any())
            {
                condicoesWhere.AppendLine(" and exists(select 1 from proposta_publico_alvo ppa where not ppa.excluido and ppa.proposta_id = p.id and ppa.cargo_funcao_id = any(@publicoAlvoIds) limit 1)");
                parametros.Add("publicoAlvoIds", filtro.PublicoAlvoIds);
            }

            if (!string.IsNullOrEmpty(filtro.NomeFormacao))
            {
                condicoesWhere.AppendLine(" and f_unaccent(p.nome_formacao) ILIKE f_unaccent(@nomeFormacao)");
                parametros.Add("nomeFormacao", $"{filtro.NomeFormacao.Trim()}%");
            }

            if (filtro.PeriodoRealizacaoInicio.HasValue)
            {
                condicoesWhere.AppendLine(" and data_realizacao_inicio::date >= @periodoRealizacaoInicio");
                parametros.Add("periodoRealizacaoInicio", filtro.PeriodoRealizacaoInicio);
            }

            if (filtro.PeriodoRealizacaoFim.HasValue)
            {
                condicoesWhere.AppendLine(" and data_realizacao_fim::date <= @periodoRealizacaoFim");
                parametros.Add("periodoRealizacaoFim", filtro.PeriodoRealizacaoFim);
            }

            if (filtro.Situacao.GetValueOrDefault() > 0)
            {
                condicoesWhere.AppendLine(" and p.situacao = @situacao");
                parametros.Add("situacao", filtro.Situacao);
            }

            if (filtro.FormacaoHomologada.HasValue)
            {
                condicoesWhere.AppendLine(" and p.formacao_homologada = @formacaoHomologada");
                parametros.Add("formacaoHomologada", filtro.FormacaoHomologada);
            }

            if (filtro.NumeroHomologacao.HasValue)
            {
                condicoesWhere.AppendLine(" and p.numero_homologacao = @numeroHomologacao");
                parametros.Add("numeroHomologacao", filtro.NumeroHomologacao);
            }

            if (filtro.PerfilUsuarioLogado.EhPerfilParecerista())
            {
                condicoesWhere.AppendLine(@" and p.situacao = @situacaoAguardandoParecerista 
                                    and p.id in (select proposta_id 
                                                 from proposta_parecerista 
                                                 where not excluido 
                                                   and registro_funcional = @loginUsuarioLogado)");
                parametros.Add("situacaoAguardandoParecerista", SituacaoProposta.AguardandoAnalisePeloParecerista);
                parametros.Add("loginUsuarioLogado", filtro.LoginUsuarioLogado);
            }

            if (filtro.Revalidacao is not null)
            {
                condicoesWhere.AppendLine(" and p.revalidacao = @revalidacao");
                parametros.Add("revalidacao", filtro.Revalidacao);
            }

            var conn = conexao.Obter();
            var sqlCount = $"SELECT COUNT(1) {sqlBaseJoins} {condicoesWhere}";

            var totalRegistros = await conn.QueryFirstAsync<int>(sqlCount, parametros);
            if (totalRegistros == 0)
                return new ResultadoPaginado<Proposta>
                {
                    Itens = [],
                    PaginaAtual = filtro.Pagina,
                    TamanhoPagina = filtro.TamanhoPagina,
                    TotalRegistros = totalRegistros
                };

            var registrosIgnorados = (filtro.Pagina - 1) * filtro.TamanhoPagina;
            parametros.Add("limite", filtro.TamanhoPagina);
            parametros.Add("registrosIgnorados", registrosIgnorados);

            var sqlConsulta = $"""
                SELECT p.id, 
                       p.tipo_formacao, 
                       p.formato, 
                       p.nome_formacao, 
                       p.data_realizacao_inicio, 
                       p.data_realizacao_fim, 
                       p.situacao, 
                       p.formacao_homologada, 
                       p.numero_homologacao, 
                       p.revalidacao, 
                       p.justificativa_revalidacao, 
                       ap.id, -- Split 
                       ap.nome
                {sqlBaseJoins}
                {condicoesWhere}
                {sqlBaseOrderBy}
                LIMIT @limite OFFSET @registrosIgnorados
                """;

            var itens = await conn.QueryAsync<Proposta, AreaPromotora, Proposta>(sqlConsulta, (proposta, areaPromotora) =>
                {
                    proposta.AreaPromotora = areaPromotora;
                    return proposta;
                },
                parametros,
                splitOn: "id, id");

            return new ResultadoPaginado<Proposta>
            {
                Itens = itens,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina,
                TotalRegistros = totalRegistros
            };
        }

        public Task<IEnumerable<Proposta>> ObterPropostasIdsDashBoard(long? areaPromotoraIdUsuarioLogado, long? propostaId, long? areaPromotoraId,
            Formato? formato, long[]? publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio,
            DateTime? periodoRealizacaoFim, SituacaoProposta? situacao, bool? formacaoHomologada, IEnumerable<SituacaoProposta> situacoesProposta,
            string loginUsuarioLogado, Guid perfilUsuarioLogado)
        {
            var query = @" 
                        with movimentacoes as (
	                        select 
		                        ROW_NUMBER() OVER(PARTITION BY proposta_id ORDER BY pm.criado_em DESC) AS Linha,
		                        pm.proposta_id, 
		                        pm.criado_em,
                                pm.situacao
	                        from proposta_movimentacao pm
	                        where not pm.excluido and pm.situacao = any(@situacoesProposta)
                        )

                        SELECT p.id, p.situacao
                        FROM proposta p
                        LEFT join movimentacoes pm on p.id = pm.proposta_id and pm.situacao = p.situacao and pm.linha = 1
                        WHERE not p.excluido and p.situacao = any(@situacoesProposta)";

            if (areaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
                query += " and p.area_promotora_id = @areaPromotoraIdUsuarioLogado ";

            if (propostaId.GetValueOrDefault() > 0)
                query += " and p.id = @propostaId ";

            if (areaPromotoraId.GetValueOrDefault() > 0)
                query += " and p.area_promotora_id = @areaPromotoraId";

            if (formato.GetValueOrDefault() > 0)
                query += " and p.formato = @formato";

            if (publicoAlvoIds.PossuiElementos())
                query += " and exists(select 1 from proposta_publico_alvo ppa where not ppa.excluido and ppa.proposta_id = p.id and ppa.cargo_funcao_id = any(@publicoAlvoIds) limit 1)";

            if (!string.IsNullOrEmpty(nomeFormacao))
            {
                nomeFormacao = "%" + nomeFormacao.ToLower() + "%";
                query += " and lower(p.nome_formacao) like @nomeFormacao";
            }

            if (periodoRealizacaoInicio.HasValue)
                query += " and p.data_realizacao_inicio::date >= @periodoRealizacaoInicio";

            if (periodoRealizacaoFim.HasValue)
                query += " and p.data_realizacao_fim::date <= @periodoRealizacaoFim";

            if (situacao.GetValueOrDefault() > 0)
                query += " and p.situacao = @situacao";

            if (formacaoHomologada.HasValue)
                query += " and p.formacao_homologada = @formacaoHomologada ";

            if (numeroHomologacao.HasValue)
                query += " and p.numero_homologacao = @numeroHomologacao ";

            if (perfilUsuarioLogado.EhPerfilParecerista())
                query += @" and p.situacao = ANY(@situacaoAguardandoParecerista) 
                            and p.id in (select proposta_id from proposta_parecerista where not excluido and registro_funcional = @loginUsuarioLogado and situacao <> @situacaoDesativado)";

            query += " ORDER BY coalesce(pm.criado_em, p.alterado_em, p.criado_em) DESC ";

            var parametros = new
            {
                areaPromotoraIdUsuarioLogado,
                propostaId,
                formato,
                publicoAlvoIds,
                nomeFormacao,
                areaPromotoraId,
                numeroHomologacao,
                periodoRealizacaoInicio = periodoRealizacaoInicio.GetValueOrDefault(),
                periodoRealizacaoFim = periodoRealizacaoFim.GetValueOrDefault(),
                situacao,
                formacaoHomologada,
                situacoesProposta = situacoesProposta.Select(t => (int)t).ToArray(),
                situacaoAguardandoParecerista = new[] { (int)SituacaoProposta.AguardandoAnalisePeloParecerista, (int)SituacaoProposta.AguardandoReanalisePeloParecerista },
                loginUsuarioLogado,
                situacaoDesativado = SituacaoParecerista.Desativado
            };
            return conexao.Obter().QueryAsync<Proposta>(query, parametros);
        }

        public async Task<IEnumerable<Proposta>> ObterPropostasDashBoard(long[] propostasIds)
        {
            var sql = @"
                        with movimentacoes as (
	                        select 
		                        ROW_NUMBER() OVER(PARTITION BY proposta_id ORDER BY pm.criado_em DESC) AS Linha,
		                        pm.*
	                        from proposta_movimentacao pm
	                        where not pm.excluido and pm.proposta_id = any(@propostasIds)
                        )

                        SELECT p.*, pm.* 
                        FROM proposta p 
                        LEFT JOIN movimentacoes pm ON p.id = pm.proposta_id AND p.situacao = pm.situacao AND pm.linha = 1
                        WHERE NOT p.excluido
                          AND p.id = any(@propostasIds)
                        ORDER BY coalesce(pm.criado_em, p.alterado_em, p.criado_em) DESC ";

            return await conexao.Obter().QueryAsync<Proposta, PropostaMovimentacao, Proposta>(sql.ToString(), (proposta, movimentacao) =>
                {
                    proposta.Movimentacao = movimentacao;
                    return proposta;
                },
                new
                {
                    propostasIds
                },
                splitOn: "id, id");
        }

        public async Task<PropostaEncontro> ObterEncontroPorId(long encontroId)
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
                        where id = @encontroId and not excluido";
            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaEncontro>(query, new { encontroId });
        }

        public async Task<PropostaRegente> ObterPropostaRegentePorId(long id)
        {
            var query = @"SELECT
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
                            cpf,
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
	                        where not excluido and id = @id;";
            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaRegente>(query, new { id });
        }

        public async Task<IEnumerable<PropostaPareceristaConsideracao>> ObterPropostaPareceristaConsideracaoPorId(long id)
        {
            var query = $@"
            select 
              ppc.id, 
              ppc.proposta_parecerista_id, 
              ppc.campo,
              ppc.descricao,
              ppc.excluido,
              ppc.criado_em,
              ppc.criado_por,
              ppc.criado_login,
              ppc.alterado_em,    
              ppc.alterado_por,
              ppc.alterado_login
            from proposta_parecerista_consideracao ppc
              join proposta_parecerista pp on pp.id = ppc.proposta_parecerista_id
            where not ppc.excluido                                                     
              and not pp.excluido
              and pp.proposta_id = @id  ";

            return await conexao.Obter().QueryAsync<PropostaPareceristaConsideracao>(query, new { id });
        }

        public async Task<PropostaTutor> ObterPropostaTutorPorId(long id)
        {
            var query = @"select
	                            id,
	                            proposta_id,
	                            profissional_rede_municipal,
	                            registro_funcional,
                                cpf,
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
	                        where not excluido and id = @id;";
            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaTutor>(query, new { id });
        }        

        public async Task<int> ObterTotalRegentes(long propostaId)
        {
            var query = @"select count(1) from proposta_regente where not excluido and proposta_id  = @propostaId ";
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<int> ObterTotalTurmasRegentes(long propostaId)
        {
            var query = @"select count(distinct prt.turma_id)
                          from proposta_regente pr
                          join proposta_regente_turma prt on prt.proposta_regente_id = pr.id and not prt.excluido
                          where not pr.excluido 
                            and pr.proposta_id = @propostaId ";

            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<int> ObterTotalTutores(long propostaId)
        {
            var query = @"select count(1) from proposta_tutor where not excluido and proposta_id = @propostaId";
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaRegente>> ObterRegentesPaginado(int numeroPagina, int numeroRegistros, long propostaId)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var query = @"select
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
                            cpf,
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

            return await conexao.Obter().QueryAsync<PropostaRegente>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public async Task<IEnumerable<PropostaTutor>> ObterTutoresPaginado(int numeroPagina, int numeroRegistros, long propostaId)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var query = @"select
	                        id,
	                        proposta_id,
	                        profissional_rede_municipal,
	                        registro_funcional,
                            cpf,
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

            return await conexao.Obter().QueryAsync<PropostaTutor>(query, new { numeroRegistros, registrosIgnorados, propostaId });
        }

        public async Task<IEnumerable<PropostaTurma>> ObterTurmasJaExistenteParaRegente(string? registroFuncional, string? cpf, string? nomeRegente, long[] turmaIds)
        {
            var query = @"select 
                            pt.id, 
                            pt.nome
                        from proposta_turma pt 
                        inner join proposta_regente_turma prt on prt.turma_id = pt.id and not prt.excluido 
                        inner join proposta_regente pr on pr.id = prt.proposta_regente_id and not pr.excluido
                        where not pt.excluido
	                      and pt.id = any(@turmaIds)";

            if (registroFuncional.EstaPreenchido())
                query += " and pr.registro_funcional = @registroFuncional ";
            else if (cpf.EstaPreenchido())
                query += " and pr.cpf = @cpf ";
            else
                query += " and trim(pr.nome_regente) = @nomeRegente ";

            return await conexao.Obter().QueryAsync<PropostaTurma>(query.ToString(), new { nomeRegente, cpf, registroFuncional, turmaIds });
        }

        public async Task<IEnumerable<PropostaTurma>> ObterTurmasJaExistenteParaTutor(string? registroFuncional, string? cpf, string? nomeTutor, long[] turmaIds)
        {
            var query = @"select 
                            pt.id, 
                            pt.nome
                        from proposta_turma pt 
                        inner join proposta_tutor_turma ptt on ptt.turma_id = pt.id and not ptt.excluido 
                        inner join proposta_tutor ptr on ptr.id = ptt.proposta_tutor_id and not ptr.excluido
                        where not pt.excluido
	                      and pt.id = any(@turmaIds)";

            if (registroFuncional.EstaPreenchido())
                query += " and ptr.registro_funcional = @registroFuncional ";
            else if (cpf.EstaPreenchido())
                query += " and ptr.cpf = @cpf ";
            else
                query += " and trim(ptr.nome_tutor) = @nomeTutor ";

            return await conexao.Obter().QueryAsync<PropostaTurma>(query.ToString(), new { nomeTutor, cpf, registroFuncional, turmaIds });
        }

        public async Task<int> AtualizarSituacao(long id, SituacaoProposta situacaoProposta)
        {
            var query = @"update proposta 
                          set 
                            situacao = @situacaoProposta, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return await conexao.Obter().ExecuteAsync(query, new
            {
                id,
                situacaoProposta,
                AlteradoEm = DateTimeExtension.HorarioBrasilia(),
                AlteradoPor = contexto.NomeUsuario,
                AlteradoLogin = contexto.UsuarioLogado
            });
        }

        public async Task<int> AtualizarSituacaoGrupoGestao(long id, SituacaoProposta situacaoProposta, long grupoGestaoId)
        {
            var query = @"update proposta 
                          set 
                            situacao = @situacaoProposta, 
                            grupo_gestao_id = @grupoGestaoId,
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return await conexao.Obter().ExecuteAsync(query, new
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

        public async Task InserirPareceristas(long id, IEnumerable<PropostaParecerista> pareceristas, SituacaoParecerista situacaoParecerista)
        {
            foreach (var parecerista in pareceristas)
            {
                PreencherAuditoriaCriacao(parecerista);

                parecerista.PropostaId = id;
                parecerista.Situacao = situacaoParecerista;
                parecerista.Id = (long)await conexao.Obter().InsertAsync(parecerista);
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

        public async Task<IEnumerable<PropostaPalavraChave>> ObterPalavrasChavesPorId(long id)
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
            return await conexao.Obter().QueryAsync<PropostaPalavraChave>(query, new { id });
        }

        public async Task<IEnumerable<PropostaParecerista>> ObterPareceristasPorId(long id)
        {
            var query = @"select 
                id, 
                proposta_id, 
                registro_funcional,
                nome_parecerista,
                situacao,
                justificativa,
                excluido,
                criado_em,
                criado_por,
                criado_login,
            	alterado_em,    
                alterado_por,
                alterado_login
            from proposta_parecerista 
            where proposta_id = @id and not excluido and situacao <> @situacaoDesativado";
            return await conexao.Obter().QueryAsync<PropostaParecerista>(query, new { id, situacaoDesativado = SituacaoParecerista.Desativado });
        }

        public async Task<IEnumerable<PropostaParecerista>> ObterSugestaoParecerPareceristas(long id)
        {
            var query = @"select 
                id,
                nome_parecerista,
                situacao,
                justificativa
            from proposta_parecerista 
            where proposta_id = @id and not excluido ";
            return await conexao.Obter().QueryAsync<PropostaParecerista>(query, new { id });
        }

        public async Task<IEnumerable<PropostaModalidade>> ObterModalidadesPorId(long id)
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
            return await conexao.Obter().QueryAsync<PropostaModalidade>(query, new { id });
        }

        public async Task<IEnumerable<PropostaAnoTurma>> ObterAnosTurmasPorId(long id)
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
            return await conexao.Obter().QueryAsync<PropostaAnoTurma>(query, new { id });
        }

        public async Task<IEnumerable<PropostaComponenteCurricular>> ObterComponentesCurricularesPorId(long id)
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
            return await conexao.Obter().QueryAsync<PropostaComponenteCurricular>(query, new { id });
        }

        public async Task<IEnumerable<PropostaCriterioCertificacao>> ObterCriterioCertificacaoPorPropostaId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaCriterioCertificacao>(query, new { propostaId });
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

        public Task RemoverParecerista(PropostaParecerista parecerista)
        {
            PreencherAuditoriaAlteracao(parecerista);

            var parametros = new
            {
                id = parecerista.Id,
                parecerista.AlteradoEm,
                parecerista.AlteradoPor,
                parecerista.AlteradoLogin
            };

            var query = @"update proposta_parecerista
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }
        public async Task AtualizarIntegrarNoSGA(long propostaId, bool valor)
        {
            var sql = "UPDATE proposta SET integrar_no_sga = @valor WHERE Id = @propostaId";
            await conexao.Obter().ExecuteAsync(sql, new { valor, propostaId });
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

        public Task RemoverPropostaMovimentacao(long propostaId)
        {
            var query = "update proposta_movimentacao set excluido = true, alterado_em = now() where proposta_id = @propostaId ";
            return conexao.Obter().ExecuteAsync(query, new { propostaId });
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

            var regenteTurmas = await queryMultiple.ReadAsync<PropostaRegenteTurma>();
            var turmas = await queryMultiple.ReadAsync<PropostaTurma>();

            foreach (var regenteTurma in regenteTurmas)
                regenteTurma.Turma = turmas.FirstOrDefault(t => t.Id == regenteTurma.TurmaId);

            return regenteTurmas;
        }

        public async Task InserirPropostaTutor(long propostaId, PropostaTutor tutor)
        {
            PreencherAuditoriaCriacao(tutor);
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
            await conexao.Obter().UpdateAsync(propostaRegente);
        }

        public async Task AtualizarPropostaTutor(PropostaTutor propostaTutor)
        {
            PreencherAuditoriaAlteracao(propostaTutor);
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


            var tutorTurmas = await queryMultiple.ReadAsync<PropostaTutorTurma>();
            var turmas = await queryMultiple.ReadAsync<PropostaTurma>();

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

        public async Task<IEnumerable<PropostaDre>> ObterDrePorId(long propostaId)
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
            return await conexao.Obter().QueryAsync<PropostaDre>(query, new { propostaId });
        }

        public async Task<IEnumerable<PropostaTurma>> ObterTurmasPorId(long propostaId)
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
                        where proposta_id = @propostaId and not excluido order by nome";
            return await conexao.Obter().QueryAsync<PropostaTurma>(query, new { propostaId });
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
                            d.dre_id as Codigo,
                            d.nome,
                            d.todos
                        from proposta_turma_dre ptd
                        join dre d on d.id = ptd.dre_id and not d.excluido
                        where ptd.proposta_turma_id = any(@propostaTurmaIds) and not ptd.excluido;";

            var multiQuery = await conexao.Obter().QueryMultipleAsync(query, new { propostaTurmaIds });
            var turmaDres = await multiQuery.ReadAsync<PropostaTurmaDre>();

            var dres = await multiQuery.ReadAsync<Dre>();

            foreach (var turmaDre in turmaDres)
            {
                var dre = dres.FirstOrDefault(t => t.Id == turmaDre.DreId);
                turmaDre.Dre = dre;
                turmaDre.DreCodigo = dre?.Codigo;
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

        public async Task InserirTurma(PropostaTurma propostaTurma)
        {
            PreencherAuditoriaCriacao(propostaTurma);
            propostaTurma.Id = (long)await conexao.Obter().InsertAsync(propostaTurma);
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
                          where not excluido and id = any(@ids);

                          update proposta_encontro_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and turma_id = any(@ids);

                          -- remove os encontros vinculados as turmas excluidas caso não possua nenhuma turma ativa.  
                          update proposta_encontro
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido 
                            and exists(select 1 from proposta_encontro_turma pet where pet.proposta_encontro_id = proposta_encontro.id and pet.turma_id = any(@ids))
                            and not exists(select 1 from proposta_encontro_turma pet where not pet.excluido and pet.proposta_encontro_id = proposta_encontro.id);

                          -- remove as datas dos encontros excluidos.  
                          update proposta_encontro_data
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido 
                            and exists(select 1 from proposta_encontro pe where pe.id = proposta_encontro_data.proposta_encontro_id and pe.excluido);
                            
                          update proposta_regente_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and turma_id = any(@ids);

                          -- remove os regentes vinculados as turmas excluidas caso não possua nenhuma turma ativa.  
                          update proposta_regente
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido 
                            and exists(select 1 from proposta_regente_turma prt where prt.proposta_regente_id = proposta_regente.id and prt.turma_id = any(@ids))
                            and not exists(select 1 from proposta_regente_turma prt where not prt.excluido and prt.proposta_regente_id = proposta_regente.id);

                          update proposta_tutor_turma
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and turma_id = any(@ids);

                          -- remove os tutores vinculados as turmas excluidas caso não possua nenhuma turma ativa.  
                          update proposta_tutor
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido 
                            and exists(select 1 from proposta_tutor_turma prt where prt.proposta_tutor_id = proposta_tutor.id and prt.turma_id = any(@ids))
                            and not exists(select 1 from proposta_tutor_turma prt where not prt.excluido and prt.proposta_tutor_id = proposta_tutor.id);
                ";

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

        public async Task<ResultadoPaginado<long>> ObterListagemFormacoesPorFiltro(FiltroListaFormacaoPropostaDto filtro)
        {
            var sql = new StringBuilder();
            var parametros = new DynamicParameters();

            // 1. Configuração Básica e Paginação
            var offset = (filtro.Pagina - 1) * filtro.TamanhoPagina;

            parametros.Add("@dataAtual", DateTimeExtension.HorarioBrasilia().Date);
            parametros.Add("@situacao", SituacaoProposta.Publicada);
            parametros.Add("@tipoInscricao", new int[] { (int)TipoInscricao.Optativa, (int)TipoInscricao.Externa });

            // Parâmetros de Paginação
            parametros.Add("@offset", offset);
            parametros.Add("@tamanhoPagina", filtro.TamanhoPagina);

            // 2. CTEs de Contexto (Se necessário)
            //TODO: ESPERAR ALTERAÇÂO DA API EOL
            filtro.FiltrarPorPerfil = false;
            if (filtro.FiltrarPorPerfil)
            {
                sql.AppendLine(ObterCteContextoServidor());
                parametros.Add("@rf", filtro.RfServidor);
                sql.AppendLine(", BaseConsulta AS (");
            }
            else
                sql.AppendLine("WITH BaseConsulta AS (");

            // 3. CTE Principal (Query Base com Distinct)
            // Encapsulamos numa CTE 'BaseConsulta' para que o DISTINCT ocorra ANTES da contagem e paginação

            sql.AppendLine("""
                SELECT DISTINCT 
                       p.id,
                       p.data_realizacao_inicio,
                       p.data_realizacao_fim
                FROM proposta p
                INNER JOIN proposta_tipo_inscricao pti ON pti.proposta_id = p.id AND NOT pti.excluido
                WHERE NOT p.excluido 
                      AND pti.tipo_inscricao = ANY(@tipoInscricao)
                      AND p.situacao = @situacao 
                      AND @dataAtual BETWEEN p.data_inscricao_inicio::date AND p.data_inscricao_fim::date                
                """);

            if (filtro.FiltrarPorPerfil)
            {
                sql.AppendLine(ObterFiltrosDePermissaoServidor());
            }

            AdicionarFiltrosOpcionais(sql, parametros, filtro);

            sql.AppendLine(")"); // Fecha CTE BaseConsulta

            // 4. Select Final com Window Function para Total e Paginação
            sql.AppendLine("""
                SELECT id, COUNT(*) OVER() AS TotalGeral
                FROM BaseConsulta
                ORDER BY data_realizacao_inicio, data_realizacao_fim
                OFFSET @offset LIMIT @tamanhoPagina;
                """);

            // 5. Execução e Mapeamento
            var resultadoDapper = await conexao.Obter().QueryAsync<dynamic>(sql.ToString(), parametros);

            var listaIds = resultadoDapper.Select(x => (long)x.id).ToList();
            var totalRegistros = resultadoDapper.FirstOrDefault()?.totalgeral ?? 0;

            return new ResultadoPaginado<long>
            {
                Itens = listaIds,
                TotalRegistros = (int)totalRegistros,
                PaginaAtual = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            };
        }

        #region Helpers de Construção SQL

        private static void AdicionarFiltrosOpcionais(StringBuilder sql, DynamicParameters p, FiltroListaFormacaoPropostaDto f)
        {
            if (f.AreasPromotorasIds?.Length > 0)
            {
                sql.AppendLine(" AND p.area_promotora_id = ANY(@areasPromotorasIds) ");
                p.Add("@areasPromotorasIds", f.AreasPromotorasIds);
            }

            if (!string.IsNullOrWhiteSpace(f.Titulo))
            {
                sql.AppendLine(" AND f_unaccent(lower(p.nome_formacao)) LIKE ('%' || f_unaccent(@titulo) || '%') ");
                p.Add("@titulo", f.Titulo.Trim().ToLower());
            }

            if (f.DataInicial.HasValue && f.DataFinal.HasValue)
            {
                sql.AppendLine(""" 
                AND (
                    (p.data_realizacao_inicio::date BETWEEN @dataInicial AND @dataFinal) OR 
                    (p.data_realizacao_fim::date BETWEEN @dataInicial AND @dataFinal)
                )
                """);
                p.Add("@dataInicial", f.DataInicial.Value.Date);
                p.Add("@dataFinal", f.DataFinal.Value.Date);
            }

            if (f.FormatosIds?.Length > 0)
            {
                sql.AppendLine(" AND p.formato = ANY(@formatosIds) ");
                p.Add("@formatosIds", f.FormatosIds);
            }

            if (f.PublicosAlvosIds?.Length > 0)
            {
                sql.AppendLine("""                    
                    AND EXISTS(SELECT 1 
                                     FROM proposta_publico_alvo ppa 
                                     WHERE NOT ppa.excluido 
                                       AND ppa.proposta_id = p.id 
                                       AND ppa.cargo_funcao_id = ANY(@publicosAlvosIds)) 
                    
                    """);
                p.Add("@publicosAlvosIds", f.PublicosAlvosIds);
            }

            if (f.PalavrasChavesIds?.Length > 0)
            {
                sql.AppendLine("""                     
                    AND EXISTS(SELECT 1 
                    
                                     FROM proposta_palavra_chave ppc 
                                     WHERE NOT ppc.excluido 
                                       AND ppc.proposta_id = p.id 
                                       AND ppc.palavra_chave_id = ANY(@palavrasChavesIds)) 
                    """);
                p.Add("@palavrasChavesIds", f.PalavrasChavesIds);
            }
        }

        private static string ObterCteContextoServidor() => """
            WITH ContextoServidor AS (
                SELECT CD_MODALIDADE, CD_COMPONENTE_CURRICULAR, ano_serie
                FROM PUBLIC.ATRIBUICOES_SERVIDOR_EOL 
                WHERE CD_REGISTRO_FUNCIONAL = @rf
            ),
            ContextoCargos AS (
                SELECT DISTINCT cf.CARGO_FUNCAO_ID
                FROM PUBLIC.CARGOS_EOL ce
                INNER JOIN PUBLIC.CARGO_FUNCAO_DEPARA_EOL cf ON ce.CD_CARGO = cf.CODIGO_CARGO_EOL
                WHERE ce.CD_REGISTRO_FUNCIONAL = @rf
            ),
            ContextoFuncoes AS (
                SELECT DISTINCT CF.CARGO_FUNCAO_ID
                FROM PUBLIC.FUNCOES_ATIVIDADES_EOL AS FE 
                INNER JOIN PUBLIC.CARGO_FUNCAO_DEPARA_EOL AS CF ON FE.CD_TIPO_FUNCAO = CF.CODIGO_FUNCAO_EOL 
                WHERE FE.CD_REGISTRO_FUNCIONAL = @rf
            )
            """;

        private static string ObterFiltrosDePermissaoServidor() =>
            """
            -- Público Alvo (Cargos) OU Vaga Remanescente
            AND (
                (
                    NOT EXISTS (SELECT 1 FROM PUBLIC.PROPOSTA_PUBLICO_ALVO WHERE PROPOSTA_ID = P.ID AND NOT EXCLUIDO)
                    AND
                    NOT EXISTS (SELECT 1 FROM PUBLIC.PROPOSTA_VAGA_REMANECENTE WHERE PROPOSTA_ID = P.ID AND NOT EXCLUIDO)
                )
                OR EXISTS (
                    SELECT 1 FROM PUBLIC.PROPOSTA_PUBLICO_ALVO PPA
                    INNER JOIN ContextoCargos CC ON CC.CARGO_FUNCAO_ID = PPA.CARGO_FUNCAO_ID
                    WHERE PPA.PROPOSTA_ID = P.ID AND NOT PPA.EXCLUIDO
                )
                OR EXISTS (
                    SELECT 1 FROM PUBLIC.PROPOSTA_VAGA_REMANECENTE PVR
                    INNER JOIN ContextoCargos CC ON CC.CARGO_FUNCAO_ID = PVR.CARGO_FUNCAO_ID
                    WHERE PVR.PROPOSTA_ID = P.ID AND NOT PVR.EXCLUIDO
                )
            )            
            --  Etapa/Modalidade
            AND (
                NOT EXISTS (SELECT 1 FROM PUBLIC.PROPOSTA_MODALIDADE WHERE PROPOSTA_ID = P.ID AND NOT EXCLUIDO)
                OR EXISTS (
                    SELECT 1 FROM PUBLIC.PROPOSTA_MODALIDADE PM
                    INNER JOIN ContextoServidor CS ON CS.CD_MODALIDADE = PM.MODALIDADE
                    WHERE PM.PROPOSTA_ID = P.ID AND NOT PM.EXCLUIDO
                )
            )
            -- Componente Curricular
            AND (
                NOT EXISTS (SELECT 1 FROM PUBLIC.PROPOSTA_COMPONENTE_CURRICULAR WHERE PROPOSTA_ID = P.ID AND NOT EXCLUIDO AND componente_curricular_id <> 1)
                OR EXISTS (
                    SELECT 1 FROM PUBLIC.PROPOSTA_COMPONENTE_CURRICULAR PCC
                    INNER JOIN ContextoServidor CS ON CS.CD_COMPONENTE_CURRICULAR = PCC.COMPONENTE_CURRICULAR_ID
                    WHERE PCC.PROPOSTA_ID = P.ID AND NOT PCC.EXCLUIDO
                )
            )
            -- Ano/Série
            AND (
                NOT EXISTS (SELECT 1 
                       FROM PUBLIC.PROPOSTA_ANO_TURMA PAT 
                       INNER JOIN PUBLIC.ANO_TURMA AT ON AT.ID = PAT.ANO_TURMA_ID
                       WHERE PAT.PROPOSTA_ID = P.ID AND AT.TODOS = false AND NOT PAT.EXCLUIDO AND NOT AT.EXCLUIDO)
                OR EXISTS (
                    SELECT 1 FROM PUBLIC.PROPOSTA_ANO_TURMA PAT
                    INNER JOIN PUBLIC.ANO_TURMA AT ON AT.ID = PAT.ANO_TURMA_ID
                    INNER JOIN ContextoServidor CS ON CS.ano_serie = AT.CODIGO_EOL
                    WHERE PAT.PROPOSTA_ID = P.ID AND NOT PAT.EXCLUIDO AND NOT AT.EXCLUIDO
                )
            ) 
            -- Função
            AND (
                NOT EXISTS (SELECT 1 FROM PUBLIC.PROPOSTA_FUNCAO_ESPECIFICA AS PFE WHERE PROPOSTA_ID = P.ID AND NOT PFE.EXCLUIDO)
                OR EXISTS (
                    SELECT 1
                    FROM PUBLIC.PROPOSTA_FUNCAO_ESPECIFICA AS PFE
                    INNER JOIN ContextoFuncoes CF ON CF.CARGO_FUNCAO_ID = PFE.CARGO_FUNCAO_ID
                    WHERE PROPOSTA_ID = P.ID AND NOT PFE.EXCLUIDO
                )
            )            
            """;

        #endregion

        public async Task<IEnumerable<Proposta>> ObterPropostasResumidasPorId(long[] propostaIds)
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
                    where not a.excluido and exists(select 1 from proposta p where a.id = p.arquivo_imagem_divulgacao_id and p.id = any(@propostaIds));";

            var multiQuery = await conexao.Obter().QueryMultipleAsync(query, new { propostaIds });

            var propostas = await multiQuery.ReadAsync<Proposta>();
            var areasPromotora = await multiQuery.ReadAsync<AreaPromotora>();
            var arquivos = await multiQuery.ReadAsync<Arquivo>();


            foreach (var proposta in propostas)
            {
                proposta.AreaPromotora = areasPromotora.FirstOrDefault(t => t.Id == proposta.AreaPromotoraId);
                proposta.ArquivoImagemDivulgacao = arquivos.FirstOrDefault(t => t.Id == proposta.ArquivoImagemDivulgacaoId);
            }

            return propostas;
        }

        public async Task<IEnumerable<PropostaPublicoAlvo>> ObterPropostasPublicoAlvoPorIdProposta(long propostaId)
        {
            var query = $@"select * from proposta_publico_alvo where proposta_id = @propostaId and excluido is not true";

            return await conexao.Obter().QueryAsync<PropostaPublicoAlvo>(query, new { propostaId });
        }
        public async Task<FormacaoDetalhada> ObterFormacaoDetalhadaPorId(long propostaId)
        {
            var tipoInscricao = new int[] { (int)TipoInscricao.Optativa, (int)TipoInscricao.Externa };
            var situacao = SituacaoProposta.Publicada;

            var query = @"select
                            p.nome_formacao NomeFormacao,
                            p.tipo_formacao tipoFormacao,
                            p.formato,
                            p.data_realizacao_inicio dataRealizacaoInicio,
                            p.data_realizacao_fim dataRealizacaoFim,                            
                            p.data_inscricao_inicio dataInscricaoInicio,
                            p.data_inscricao_fim dataInscricaoFim,
                            p.justificativa,
                            p.formacao_homologada as FormacaoHomologada,
                            p.link_inscricoes_externa as LinkParaInscricoesExterna,
                            coalesce((select false from public.proposta_tipo_inscricao pti where pti.proposta_id = p.id and pti.tipo_inscricao = 5), true) as PodeEnviarInscricao
                        from proposta p
                        inner join proposta_tipo_inscricao pti on pti.proposta_id = p.id
                        where p.id = @propostaId 
                            and not p.excluido
                            and pti.tipo_inscricao = any(@tipoInscricao) 
                            and p.situacao = @situacao;

                          select
                              ap.nome
                        from proposta p 
                            join area_promotora ap on ap.id = p.area_promotora_id
                        where p.id = @propostaId 
                            and not p.excluido 
                            and not ap.excluido;

                        select 
                               case when cf.outros then p.publico_alvo_outros else cf.nome end as nome
                        from proposta_publico_alvo ppa
                        join cargo_funcao cf on cf.id = ppa.cargo_funcao_id
                        join proposta p on p.id = ppa.proposta_id and not p.excluido
                        where ppa.proposta_id = @propostaId
                          and not ppa.excluido 
                          and not cf.excluido;

                        select 
                               cf.nome
                        from proposta_vaga_remanecente pvr
                        join cargo_funcao cf on cf.id = pvr.cargo_funcao_id
                        where pvr.proposta_id = @propostaId
                          and not pvr.excluido
                          and not cf.excluido;

                        select distinct pt.id,
                               pt.nome,
                               pe.local,
                               pe.hora_inicio horaInicio,
                               pe.hora_fim horaFim,
                               pet.proposta_encontro_id propostaEncontroId,
                               coalesce(PGP.DATA_INICIO, p.data_realizacao_inicio) AS dataInicio,
                               coalesce(PGP.DATA_FIM, p.data_realizacao_fim) AS dataFim
                        from proposta_turma pt
                        join proposta_encontro_turma pet on pet.turma_id = pt.id
                        join proposta_encontro pe on pe.id = pet.proposta_encontro_id
                        JOIN proposta p ON p.ID = pt.PROPOSTA_ID
                        LEFT JOIN proposta_grupo_periodo_turma pgpt ON pgpt.PROPOSTA_TURMA_ID = pt.ID AND NOT pgpt.EXCLUIDO 
                        LEFT JOIN PUBLIC.PROPOSTA_GRUPO_PERIODO AS PGP ON PGP.ID = pgpt.GRUPO_PERIODO_ID AND NOT pgp.EXCLUIDO
                        where pt.proposta_id = @propostaId
                          and not pt.excluido
                          and not pet.excluido
                          and not pe.excluido
                        order by pt.nome, dataInicio, pe.hora_inicio;

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

                        select
                              coalesce(pgp.data_inicio, ped.data_inicio) dataInicio,
                              coalesce(pgp.data_fim, ped.data_fim) dataFim,
                              ped.proposta_encontro_id propostaEncontroId,
                              coalesce(ped.hora_inicio, pe.hora_inicio) horaInicio,
                              coalesce(ped.hora_fim, pe.hora_fim) horaFim,
                              case when ped.hora_inicio is not null then 'novo' else 'legado' end modeloHorario
                        from proposta_encontro pe
                        join proposta_encontro_data ped on ped.proposta_encontro_id = pe.id and not ped.excluido
                        left join proposta_encontro_turma pet on pet.proposta_encontro_id = pe.id and not pet.excluido
                        left join proposta_grupo_periodo_turma pgpt on pgpt.proposta_turma_id = pet.turma_id and not pgpt.excluido
                        left join proposta_grupo_periodo pgp on pgp.id = pgpt.grupo_periodo_id and not pgp.excluido
                        where pe.proposta_id = @propostaId
                          and not pe.excluido
                        order by dataInicio;

                        select a.nome,
                               a.codigo
                        from arquivo a
                        where not a.excluido and exists(select 1 from proposta p where not p.excluido and a.id = p.arquivo_imagem_divulgacao_id and p.id = @propostaId);";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { propostaId, tipoInscricao, situacao });

            var formacaoDetalhe = await queryMultiple.ReadFirstOrDefaultAsync<FormacaoDetalhada>();

            if (formacaoDetalhe.NaoEhNulo())
            {
                formacaoDetalhe.AreaPromotora = await queryMultiple.ReadFirstOrDefaultAsync<string>();
                formacaoDetalhe.PublicosAlvo = await queryMultiple.ReadAsync<string>();  
                formacaoDetalhe.PalavrasChaves = await queryMultiple.ReadAsync<string>();
                formacaoDetalhe.Turmas = await queryMultiple.ReadAsync<FormacaoTurma>();
                var formacaoDatasTurmas = await queryMultiple.ReadAsync<FormacaoTurmaData>();
                var formacaoDatasTurmasNovo = await queryMultiple.ReadAsync<FormacaoTurmaDataNovo>();
                var arquivos = await queryMultiple.ReadAsync<Arquivo>();
                formacaoDetalhe.ArquivoImagemDivulgacao = arquivos.Any() ? arquivos.FirstOrDefault() : null;

                foreach (var turma in formacaoDetalhe.Turmas)
                {
                    turma.Periodos = formacaoDatasTurmas.Where(w => w.PropostaEncontroId == turma.PropostaEncontroId).OrderBy(o => o.DataInicio);
                    turma.DatasNovo = formacaoDatasTurmasNovo.Where(w => w.PropostaEncontroId == turma.PropostaEncontroId).OrderBy(o => o.DataInicio);
                }

                formacaoDetalhe.Turmas = formacaoDetalhe.Turmas.OrderBy(o => o.Periodos.FirstOrDefault().DataInicio);
            }

            return formacaoDetalhe;
        }

        public async Task<int> InserirPropostaTurmaVagas(PropostaTurmaVaga propostaTurmaVaga, int quantidade)
        {
            PreencherAuditoriaCriacao(propostaTurmaVaga);

            var insert = "insert into proposta_turma_vaga (proposta_turma_id, criado_em, criado_por, criado_login) values (@PropostaTurmaId, @CriadoEm, @CriadoPor, @CriadoLogin);";

            var inserts = new StringBuilder();

            for (int i = 0; i < quantidade; i++)
                inserts.AppendLine(insert);

            return await conexao.Obter().ExecuteAsync(inserts.ToString(), propostaTurmaVaga);
        }

        public async Task<PropostaTurma> ObterTurmaPorId(long propostaTurmaId)
        {
            return await conexao.Obter().GetAsync<PropostaTurma>(propostaTurmaId);
        }

        public async Task<PropostaTurma?> ObterTurmaNaoExcluidaPorIdAsync(long propostaTurmaId)
        {
            return await conexao.Obter().FirstOrDefaultAsync<PropostaTurma>(PropostaTurma => PropostaTurma.Id == propostaTurmaId && !PropostaTurma.Excluido);
        }

        public async Task<IEnumerable<PropostaTurma>> ObterTurmasComVagaPorId(long propostaId, string? codigoDre = null)
        {
            var query = $@"select 
                            pt.id, 
                            pt.proposta_id, 
                            pt.nome
                        from proposta_turma pt
                        where pt.proposta_id = @propostaId 
                          and not pt.excluido
                          and exists(select 1 
                                     from proposta_turma_vaga ptv 
                                     inner join proposta_turma_dre ptd on ptv.proposta_turma_id = ptd.proposta_turma_id
                                     inner join dre on ptd.dre_id = dre.id
                                     where ptv.proposta_turma_id = pt.id 
                                       and not ptv.excluido 
                                       and ptv.inscricao_id is null
                                       {(codigoDre is not null ? "and dre.dre_id = @codigoDre" : string.Empty)})";

            return await conexao.Obter().QueryAsync<PropostaTurma>(query, new { propostaId, codigoDre });
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

            var encontros = await multiquery.ReadAsync<PropostaEncontro>();
            var datas = await multiquery.ReadAsync<PropostaEncontroData>();

            foreach (var encontro in encontros)
                encontro.Datas = datas.Where(t => t.PropostaEncontroId == encontro.Id);

            return encontros;
        }

        public async Task<PropostaInscricaoAutomatica> ObterPropostaInscricaoPorId(long propostaId)
        {
            var query = @"
            select p.id as propostaId,
                   p.integrar_no_sga as IntegrarNoSGA,
                   p.situacao,
                   p.quantidade_vagas_turma as QuantidadeVagasTurmas
            from proposta p
            where p.id = @propostaId and not p.excluido;

            select pti.tipo_inscricao
            from proposta_tipo_inscricao pti
            where pti.proposta_id = @propostaId and not pti.excluido;

            select pt.id,
                   ptd.dre_id as CodigoDre, 
                   dre.dre_id as codigoDre
            from proposta_turma pt
              join proposta_turma_dre ptd on ptd.proposta_turma_id = pt.id and not ptd.excluido 
              left join dre on dre.id = ptd.dre_id and not dre.excluido and not dre.todos
            where not pt.excluido 
              and pt.proposta_id = @propostaId;
              
            select distinct cfde.codigo_cargo_eol
            from proposta_publico_alvo ppa
            left join cargo_funcao_depara_eol cfde on cfde.cargo_funcao_id = ppa.cargo_funcao_id and not ppa.excluido
            where not ppa.excluido   
              and ppa.proposta_id = @propostaId;
            
            select distinct cfde.codigo_funcao_eol
            from proposta_funcao_especifica pfe
            left join cargo_funcao_depara_eol cfde on cfde.cargo_funcao_id = pfe.cargo_funcao_id and not cfde.excluido
            where not pfe.excluido
              and pfe.proposta_id = @propostaId;
            
            select distinct at.codigo_eol
            from proposta_ano_turma pat
              join ano_turma at on at.id = pat.ano_turma_id 
            where not pat.excluido
              and not at.excluido
              and not at.todos
              and proposta_id = @propostaId;
            
            select distinct cc.codigo_eol
            from proposta_componente_curricular pcc
              join componente_curricular cc on cc.id = pcc.componente_curricular_id
            where  not cc.excluido 
               and not pcc.excluido 
               and not cc.todos
               and proposta_id = @propostaId; 

            select modalidade
            from proposta_modalidade pm  
            where  not pm.excluido
               and proposta_id = @propostaId; ";

            var queryMultiple = await conexao.Obter().QueryMultipleAsync(query, new { propostaId });

            var propostaInscricaoAutomatica = await queryMultiple.ReadFirstOrDefaultAsync<PropostaInscricaoAutomatica>();
            if (propostaInscricaoAutomatica.NaoEhNulo())
            {
                propostaInscricaoAutomatica.TiposInscricao = await queryMultiple.ReadAsync<TipoInscricao>();
                propostaInscricaoAutomatica.PropostasTurmas = await queryMultiple.ReadAsync<PropostaInscricaoAutomaticaTurma>();
                propostaInscricaoAutomatica.PublicosAlvos = await queryMultiple.ReadAsync<long?>();
                propostaInscricaoAutomatica.FuncoesEspecificas = await queryMultiple.ReadAsync<long?>();
                propostaInscricaoAutomatica.AnosTurmas = await queryMultiple.ReadAsync<string>();
                propostaInscricaoAutomatica.ComponentesCurriculares = await queryMultiple.ReadAsync<long>();
                propostaInscricaoAutomatica.Modalidades = await queryMultiple.ReadAsync<long>();
            }

            return propostaInscricaoAutomatica;
        }

        public async Task<IEnumerable<PropostaRegente>> ObterRegentesPorPropostaTurmaId(long propostaTurmaId)
        {
            var query = @"SELECT
	                        pr.id,
	                        pr.proposta_id,
	                        pr.profissional_rede_municipal,
	                        pr.registro_funcional,
                            pr.cpf,
	                        pr.nome_regente,
	                        pr.mini_biografia,
	                        pr.criado_em,
	                        pr.criado_por,
	                        pr.alterado_em,
	                        pr.alterado_por,
	                        pr.criado_login,
	                        pr.alterado_login,
	                        pr.excluido
                        FROM proposta_regente_turma prt
	                    INNER JOIN proposta_regente pr on pr.id = prt.proposta_regente_id and not pr.excluido
	                    WHERE not prt.excluido and prt.turma_id = @propostaTurmaId;";

            return await conexao.Obter().QueryAsync<PropostaRegente>(query, new { propostaTurmaId });
        }

        public async Task<IEnumerable<PropostaTutor>> ObterTutoresPorPropostaTurmaId(long propostaTurmaOrigemId)
        {
            var query = @"select
	                            pt.id,
	                            pt.proposta_id,
	                            pt.profissional_rede_municipal,
	                            pt.registro_funcional,
                                pt.cpf,
	                            pt.nome_tutor,
	                            pt.criado_em,
	                            pt.criado_por,
	                            pt.alterado_em,
	                            pt.alterado_por,
	                            pt.criado_login,
	                            pt.alterado_login,
	                            pt.excluido
                            from proposta_tutor_turma ptt
	                        inner join proposta_tutor pt on pt.id = ptt.proposta_tutor_id and not pt.excluido
	                        where not ptt.excluido and ptt.turma_id = @propostaTurmaOrigemId;";
            return await conexao.Obter().QueryAsync<PropostaTutor>(query, new { propostaTurmaOrigemId });
        }

        public async Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorId(long propostaId)
        {
            var query = @"select 
                            id, 
                            proposta_id, 
                            tipo_inscricao,
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_tipo_inscricao
                        where proposta_id = @propostaId and not excluido";
            return await conexao.Obter().QueryAsync<PropostaTipoInscricao>(query, new { propostaId });
        }

        public async Task InserirTiposInscricao(long propostaId, IEnumerable<PropostaTipoInscricao> tiposInscricao)
        {
            foreach (var tipoInscricao in tiposInscricao)
            {
                PreencherAuditoriaCriacao(tipoInscricao);

                tipoInscricao.PropostaId = propostaId;
                tipoInscricao.Id = (long)await conexao.Obter().InsertAsync(tipoInscricao);
            }
        }

        public Task RemoverTiposInscricao(IEnumerable<PropostaTipoInscricao> tiposInscrocao)
        {
            var tipoInscricao = tiposInscrocao.First();
            PreencherAuditoriaAlteracao(tipoInscricao);

            var parametros = new
            {
                ids = tiposInscrocao.Select(t => t.Id).ToArray(),
                tipoInscricao.AlteradoEm,
                tipoInscricao.AlteradoPor,
                tipoInscricao.AlteradoLogin
            };

            var query = @"update proposta_tipo_inscricao
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = any(@ids)";

            return conexao.Obter().ExecuteAsync(query, parametros);
        }

        public async Task<int> ObterTotalVagasTurma(long propostaTurmaIr)
        {
            var query = "select count(1) from proposta_turma_vaga where proposta_turma_id = @propostaTurmaIr and not excluido";
            return await conexao.Obter().ExecuteScalarAsync<int>(query, new { propostaTurmaIr });
        }

        public async Task<PropostaTurma> ObterTurmaPorNome(string nome, long propostaId)
        {
            var query = @"
            SELECT id,
                   proposta_id,
                   criado_em,
                   criado_por,
                   alterado_em,
                   alterado_por,
                   criado_login,
                   alterado_login,
                   excluido bool,
                   nome
            FROM proposta_turma
            WHERE not excluido
            AND f_unaccent(lower(nome)) = f_unaccent(@nome) 
            AND proposta_id = @propostaId ";

            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaTurma>(query, new { nome = nome.ToLower(), propostaId });
        }

        public async Task<IEnumerable<long>> PropostasConfirmadasQueNaoEncerramAinda()
        {
            var dataAtual = DateTimeExtension.HorarioBrasilia();

            var query = @"select id from proposta where not excluido  and data_realizacao_fim >= @dataAtual ";

            return await conexao.Obter().QueryAsync<long>(query, new { dataAtual });
        }

        public async Task<IEnumerable<long>> PropostasTurmaIdsPorPropostaId(long propostaId)
        {
            var query = "select id from proposta_turma pt where not pt.excluido and pt.proposta_id = @propostaId ";

            return await conexao.Obter().QueryAsync<long>(query, new { propostaId });
        }

        public async Task<bool> RemoverPropostaParecer(PropostaPareceristaConsideracao pareceristaConsideracao)
        {
            PreencherAuditoriaAlteracao(pareceristaConsideracao);

            var parametros = new
            {
                pareceristaConsideracao.Id,
                pareceristaConsideracao.AlteradoEm,
                pareceristaConsideracao.AlteradoPor,
                pareceristaConsideracao.AlteradoLogin
            };

            var query = @"update proposta_parecerista_consideracao 
                          set 
                            excluido = true, 
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin 
                          where not excluido and id = @id";

            return await conexao.Obter().ExecuteAsync(query, parametros) > 0;
        }

        public async Task<PropostaPareceristaConsideracao> ObterParecerPorId(long parecerId)
        {
            var query = @" 
            select 
              id, 
              proposta_parecerista_id, 
              campo,
              descricao,
              excluido,
              criado_em,
              criado_por,
              criado_login,
              alterado_em,    
              alterado_por,
              alterado_login             
            from public.proposta_parecerista_consideracao 
            where not excluido  
              and id = @parecerId ";
            return await conexao.Obter().QueryFirstOrDefaultAsync<PropostaPareceristaConsideracao>(query, new { parecerId });
        }

        public Task<bool> ExistePareceristasAdicionadosNaProposta(long propostaId)
        {
            var query = @"select count(1) from proposta_parecerista where proposta_id = @propostaId and not excluido";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { propostaId });
        }

        public Task<bool> AvaliarSituacaoPropostaPorIdSituacao(long propostaId, SituacaoProposta situacaoProposta)
        {
            var query = @"select count(1) 
                          from proposta
                          where not excluido
                            and id = @propostaId 
                            and situacao = @situacaoProposta";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { propostaId, situacaoProposta = (int)situacaoProposta });
        }

        public async Task<IEnumerable<PropostaParecerista>> ObterPareceristasPorPropostaId(long propostaId)
        {
            var query = @" 
            select id,
	               proposta_id,
	               registro_funcional,
	               nome_parecerista,
	               situacao,
	               justificativa,
	               criado_por,
	               alterado_em,
	               alterado_por,
	               criado_login,
	               alterado_login,
	               excluido
           from public.proposta_parecerista 
           where not excluido  
                 and proposta_id = @propostaId ";
            return await conexao.Obter().QueryAsync<PropostaParecerista>(query, new { propostaId });
        }

        public Task AtualizarSituacaoParecerista(long propostaPareceristaId, string registroFuncional, SituacaoParecerista situacao, string justificativa)
        {
            var query = @"update proposta_parecerista
                          set 
                            situacao = @situacao, 
                            justificativa = @justificativa
                          where not excluido
                            and id = @propostaPareceristaId 
                            and registro_funcional = @registroFuncional";

            return conexao.Obter().ExecuteAsync(query, new { propostaPareceristaId, registroFuncional, situacao, justificativa });
        }

        public Task<PropostaParecerista> ObterPareceristaPorPropostaIdRegistroFuncional(long propostaId, string registroFuncional)
        {
            var query = $@"
            select id,
	               proposta_id,
	               registro_funcional,
	               nome_parecerista,
                   situacao,
                   justificativa, 
	               criado_em,
	               criado_por,
	               alterado_em,
	               alterado_por,
	               criado_login,
	               alterado_login,
	               excluido
            from public.proposta_parecerista
	        where not excluido 
              and proposta_id = @propostaId and registro_funcional = @registroFuncional ";

            return conexao.Obter().QueryFirstOrDefaultAsync<PropostaParecerista>(query, new { propostaId, registroFuncional });
        }

        public async Task<ResultadoPaginado<AutocompletarNumeroHomologacaoDto>> ObterAutocompletarNumeroHomologacaoAsync(string termo, int numeroPagina, int numeroRegistros)
        {
            termo = $"{termo}%";
            const string sqlBase = """
                FROM PUBLIC.PROPOSTA AS P 
                WHERE NOT P.EXCLUIDO
                  AND CAST(P.NUMERO_HOMOLOGACAO AS TEXT) ILIKE @termo
                """;
            const string sqlSelect = $"""
                SELECT p.ID AS propostaId,
                       p.NUMERO_HOMOLOGACAO AS numeroHomologacao,
                       p.NOME_FORMACAO AS nomeFormacao,
                       p.ID AS codigoFormacao
                {sqlBase}
                ORDER BY numeroHomologacao
                LIMIT @limit OFFSET @offset;
                """;
            const string sqlCount = $"SELECT COUNT(1) {sqlBase}";
            var conn = conexao.Obter();
            var totalRegistros = await conn.ExecuteScalarAsync<int>(sqlCount, new { termo });
            if (totalRegistros == 0)
                return new()
                {
                    Itens = [],
                    TotalRegistros = 0,
                    PaginaAtual = numeroPagina,
                    TamanhoPagina = numeroRegistros
                };

            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var dados = await conn.QueryAsync<AutocompletarNumeroHomologacaoDto>(sqlSelect,
                new { termo, limit = numeroRegistros, offset = registrosIgnorados });

            return new()
            {
                Itens = dados,
                TotalRegistros = totalRegistros,
                PaginaAtual = numeroPagina,
                TamanhoPagina = numeroRegistros
            };
        }
    }
}