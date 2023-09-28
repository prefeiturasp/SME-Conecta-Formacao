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

        public async Task InserirCriteriosValidacaoInscricao(long id, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaCriacao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.PropostaId = id;
                criterioValidacaoInscricao.Id = (long)await conexao.Obter().InsertAsync(criterioValidacaoInscricao);
            }
        }

        public async Task InserirFuncoesEspecificas(long id, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaCriacao(funcaoEspecifica);

                funcaoEspecifica.PropostaId = id;
                funcaoEspecifica.Id = (long)await conexao.Obter().InsertAsync(funcaoEspecifica);
            }
        }

        public async Task InserirPublicosAlvo(long id, IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            foreach (var publicoAlvo in publicosAlvo)
            {
                PreencherAuditoriaCriacao(publicoAlvo);

                publicoAlvo.PropostaId = id;
                publicoAlvo.Id = (long)await conexao.Obter().InsertAsync(publicoAlvo);
            }
        }

        public async Task InserirVagasRemanecentes(long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaCriacao(vagaRemanecente);

                vagaRemanecente.PropostaId = id;
                vagaRemanecente.Id = (long)await conexao.Obter().InsertAsync(vagaRemanecente);
            }
        }

        public Task<IEnumerable<PropostaCriterioValidacaoInscricao>> ObterCriteriosValidacaoInscricaoPorId(long id)
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaCriterioValidacaoInscricao>(query, new { id });
        }

        public Task<IEnumerable<PropostaFuncaoEspecifica>> ObterFuncoesEspecificasPorId(long id)
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaFuncaoEspecifica>(query, new { id });
        }

        public Task<IEnumerable<PropostaPublicoAlvo>> ObterPublicoAlvoPorId(long id)
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaPublicoAlvo>(query, new { id });
        }

        public Task<IEnumerable<PropostaVagaRemanecente>> ObterVagasRemacenentesPorId(long id)
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaVagaRemanecente>(query, new { id });
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

        private static string MontarQueryPaginacao(long? id, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, ref string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            var query = new StringBuilder();
            query.AppendLine("select p.*, ap.* ");
            query.AppendLine("from proposta p ");
            query.AppendLine("left join area_promotora ap on ap.id = p.area_promotora_id and not ap.excluido");
            query.AppendLine("where not p.excluido ");

            if (id.GetValueOrDefault() > 0)
                query.AppendLine(" and p.id = @id");

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

            if (situacao.GetValueOrDefault() > 0)
                query.AppendLine(" and p.situacao = @situacao");

            return query.ToString();
        }

        public Task<int> ObterTotalRegistrosPorFiltros(long? id, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            string query = string.Concat("select count(1) from (", MontarQueryPaginacao(id, areaPromotoraId, modalidade, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao), ") tb");
            return conexao.Obter().ExecuteScalarAsync<int>(query, new
            {
                id,
                areaPromotoraId,
                modalidade,
                publicoAlvoIds,
                nomeFormacao,
                numeroHomologacao,
                periodoRealizacaoInicio,
                periodoRealizacaoFim,
                situacao
            });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginados(int numeroPagina, int numeroRegistros, long? id, long? areaPromotoraId, Modalidade? modalidade, long[] publicoAlvoIds, string? nomeFormacao, long? numeroHomologacao, DateTime? periodoRealizacaoInicio, DateTime? periodoRealizacaoFim, SituacaoProposta? situacao)
        {
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            string query = MontarQueryPaginacao(id, areaPromotoraId, modalidade, publicoAlvoIds, ref nomeFormacao, numeroHomologacao, periodoRealizacaoInicio, periodoRealizacaoFim, situacao);

            query += " order by p.criado_em desc";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<Proposta, AreaPromotora, Proposta>(query, (proposta, areaPromotora) =>
            {
                proposta.AreaPromotora = areaPromotora;
                return proposta;
            },
            new
            {
                id,
                numeroRegistros,
                registrosIgnorados,
                areaPromotoraId,
                modalidade,
                publicoAlvoIds,
                nomeFormacao,
                numeroHomologacao,
                periodoRealizacaoInicio,
                periodoRealizacaoFim,
                situacao
            },
            splitOn: "id, id");
        }

        public Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorId(long id)
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaEncontro>(query, new { id });
        }

        public async Task InserirEncontros(long id, IEnumerable<PropostaEncontro> encontros)
        {
            foreach (var encontro in encontros)
            {
                PreencherAuditoriaCriacao(encontro);

                encontro.PropostaId = id;
                encontro.Id = (long)await conexao.Obter().InsertAsync(encontro);
            }
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

        public Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroIds(long[] encontroIds)
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

        public Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroIds(long[] encontroIds)
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

        public Task<int> ObterTotalEncontros(long propostaId)
        {
            var query = @"select count(1) from proposta_encontro where not excluido and proposta_id = @propostaId";
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
                            excluido,
                            criado_em,
	                        criado_por,
                            criado_login,
                        	alterado_em,    
	                        alterado_por,
	                        alterado_login
                        from proposta_encontro 
                        where proposta_id = @id";

            query += " order by id";
            query += " limit @numeroRegistros offset @registrosIgnorados";

            return conexao.Obter().QueryAsync<PropostaEncontro>(query, new { numeroRegistros, registrosIgnorados, propostaId });
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
                        where proposta_id = @id";
            return conexao.Obter().QueryAsync<PropostaPalavraChave>(query, new { id });
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
    }
}
