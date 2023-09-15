using Dapper;
using Dommel;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioProposta : RepositorioBaseAuditavel<Proposta>, IRepositorioProposta
    {
        public RepositorioProposta(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> Atualizar(IDbTransaction transacao, Proposta proposta)
        {
            PreencherAuditoriaAlteracao(proposta);
            return await conexao.Obter().UpdateAsync(proposta, transacao);
        }

        public async Task RemoverCriteriosValidacaoInscricao(IDbTransaction transacao, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaAlteracao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.Excluido = true;
                await conexao.Obter().UpdateAsync(criterioValidacaoInscricao, transacao);
            }
        }

        public async Task RemoverVagasRemanecentes(IDbTransaction transacao, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaAlteracao(vagaRemanecente);
                vagaRemanecente.Excluido = true;
                await conexao.Obter().UpdateAsync(vagaRemanecente, transacao);
            }
        }

        public async Task<long> Inserir(IDbTransaction transacao, Proposta proposta)
        {
            PreencherAuditoriaCriacao(proposta);
            proposta.Id = (long)await conexao.Obter().InsertAsync(proposta, transacao);
            return proposta.Id;
        }

        public async Task InserirCriteriosValidacaoInscricao(IDbTransaction transacao, long id, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            foreach (var criterioValidacaoInscricao in criteriosValidacaoInscricao)
            {
                PreencherAuditoriaCriacao(criterioValidacaoInscricao);
                criterioValidacaoInscricao.PropostaId = id;
                criterioValidacaoInscricao.Id = (long)await conexao.Obter().InsertAsync(criterioValidacaoInscricao, transacao);
            }
        }

        public async Task InserirFuncoesEspecificas(IDbTransaction transacao, long id, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaCriacao(funcaoEspecifica);

                funcaoEspecifica.PropostaId = id;
                funcaoEspecifica.Id = (long)await conexao.Obter().InsertAsync(funcaoEspecifica, transacao);
            }
        }

        public async Task InserirPublicosAlvo(IDbTransaction transacao, long id, IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            foreach (var publicoAlvo in publicosAlvo)
            {
                PreencherAuditoriaCriacao(publicoAlvo);

                publicoAlvo.PropostaId = id;
                publicoAlvo.Id = (long)await conexao.Obter().InsertAsync(publicoAlvo, transacao);
            }
        }

        public async Task InserirVagasRemanecentes(IDbTransaction transacao, long id, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                PreencherAuditoriaCriacao(vagaRemanecente);

                vagaRemanecente.PropostaId = id;
                vagaRemanecente.Id = (long)await conexao.Obter().InsertAsync(vagaRemanecente, transacao);
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

        public async Task RemoverFuncoesEspecificas(IDbTransaction transacao, IEnumerable<PropostaFuncaoEspecifica> funcoesEspecificas)
        {
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                PreencherAuditoriaAlteracao(funcaoEspecifica);
                funcaoEspecifica.Excluido = true;
                await conexao.Obter().UpdateAsync(funcaoEspecifica, transacao);
            }
        }

        public async Task RemoverPublicosAlvo(IDbTransaction transacao, IEnumerable<PropostaPublicoAlvo> publicoAlvo)
        {
            foreach (var publico in publicoAlvo)
            {
                PreencherAuditoriaAlteracao(publico);
                publico.Excluido = true;
                await conexao.Obter().UpdateAsync(publico, transacao);
            }
        }

        public async Task Remover(IDbTransaction transacao, Proposta proposta)
        {
            PreencherAuditoriaAlteracao(proposta);
            proposta.Excluido = true;
            await conexao.Obter().UpdateAsync(proposta, transacao);
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
    }
}
