using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioInscricao : RepositorioBaseAuditavel<Inscricao>, IRepositorioInscricao
    {
        public RepositorioInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> ConfirmarInscricaoVaga(Inscricao inscricao)
        {
            PreencherAuditoriaAlteracao(inscricao);

            var query = @"
                        with vaga as (
	                        select id 
	                        from proposta_turma_vaga 
	                        where proposta_turma_id = @PropostaTurmaId
	                        and inscricao_id is null limit 1 for update skip locked
                        )

                        update proposta_turma_vaga set  
                            inscricao_id = @Id,
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin
                          where id = (select id from vaga)";

            return await conexao.Obter().ExecuteAsync(query, inscricao) > 0;
        }

        public Task<bool> UsuarioEstaInscritoNaProposta(long propostaId, long usuarioId)
        {
            var situacaoCancelada = (int)SituacaoInscricao.Cancelada;

            var query = @"
                        select 1 
                        from proposta_turma pt 
                        left join inscricao i on pt.id = i.proposta_turma_id and not i.excluido
                        where pt.proposta_id = @propostaId 
	                        and i.usuario_id = @usuarioId 
	                        and i.situacao <> @situacaoCancelada
                        limit 1";

            return conexao.Obter().ExecuteScalarAsync<bool>(query, new { propostaId, usuarioId, situacaoCancelada });
        }

        public Task<int> LiberarInscricaoVaga(Inscricao inscricao)
        {
            PreencherAuditoriaAlteracao(inscricao);

            var query = @"update proposta_turma_vaga set
                            inscricao_id = null,
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin
                          where proposta_turma_id = @PropostaTurmaId
                          and inscricao_id = @InscricaoId ";

            return conexao.Obter().ExecuteAsync(query,
                new
                {
                    inscricao.AlteradoEm,
                    inscricao.AlteradoPor,
                    inscricao.AlteradoLogin,
                    inscricao.PropostaTurmaId,
                    InscricaoId = inscricao.Id,
                });
        }

        public Task<CargoFuncaoDTO> ObterCargoFuncaoPorId(long id)
        {
            const string query = @"
                                    select
                                        case
                                            when i.funcao_id is not null then i.funcao_codigo
                                            when i.cargo_id is not null then i.cargo_codigo
                                            else ''
                                        end as CargoFuncaoCodigo,
                                        case
                                            when i.tipo_vinculo is not null then
                                                case
                                                    when i.funcao_id is not null then trim(cff.nome) || ' - v' || cast(i.tipo_vinculo as varchar(10))
                                                    when i.cargo_id is not null then trim(cfc.nome) || ' - v' || cast(i.tipo_vinculo as varchar(10))
                                                    else ''
                                                end
                                            else
                                                case
                                                    when i.funcao_id is not null then cff.nome
                                                    when i.cargo_id is not null then cfc.nome
                                                    else ''
                                                end
                                        end as CargoFuncaoNome,
                                        i.tipo_vinculo as TipoVinculo
                                    from inscricao i
                                    left join cargo_funcao cfc on cfc.id = i.cargo_id
                                    left join cargo_funcao cff on cff.id = i.funcao_id
                                    where i.id = @id
                                    ";

            return conexao.Obter().QueryFirstOrDefaultAsync<CargoFuncaoDTO>(query, new { id });
        }

        public Task<IEnumerable<Inscricao>> ObterDadosPaginadosPorUsuarioId(long usuarioId, int numeroPagina, int numeroRegistros)
        {
            var query = @"
                select i.id,
                       i.situacao,
                       i.origem, 
                       i.proposta_turma_id,
                       pt.nome,
                       pt.proposta_id,
                       p.nome_formacao,
                       p.data_realizacao_inicio,
                       p.data_realizacao_fim,
                       p.id as Id,
                       p.integrar_no_sga 
                from inscricao i 
                inner join proposta_turma pt on pt.id = i.proposta_turma_id 
                inner join proposta p on p.id = pt.proposta_id 
                where not i.excluido 
                    and not p.excluido 
	                and i.usuario_id = @usuarioId
                order by i.id
                limit @numeroRegistros offset @registrosIgnorados";

            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            return conexao.Obter().QueryAsync<Inscricao, PropostaTurma, Proposta, Inscricao>(query, (inscricao, propostaTurma, proposta) =>
            {
                propostaTurma.Proposta = proposta;
                inscricao.PropostaTurma = propostaTurma;
                return inscricao;
            }, new { usuarioId, numeroRegistros, registrosIgnorados }, splitOn: "id, proposta_turma_id, proposta_id");
        }

        public Task<int> ObterTotalRegistrosPorUsuarioId(long usuarioId)
        {
            var query = @$"select count(1) 
                           from inscricao i 
                             join proposta_turma pt on pt.id = i.proposta_turma_id 
                             join proposta p on p.id = pt.proposta_id 
                           where not i.excluido 
                                and not p.excluido 
                                and usuario_id = @usuarioId ";
            return conexao.Obter().ExecuteScalarAsync<int>(query, new { usuarioId });
        }

        public Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long propostaId, string? login, string? cpf, string? nomeCursista, long[]? turmasId,
            int numeroPagina, int numeroRegistros)
        {
            var query = new StringBuilder(@"select 
                                                i.id,
												i.situacao,
                                                i.origem,
                                                i.proposta_turma_id,
												pt.nome,
                                                i.usuario_id ,
												u.login,
												u.cpf ,
												u.nome,
                                                i.cargo_id,
                                                i.funcao_id,
												case 
													when i.tipo_vinculo is not null then trim(cf.nome) || ' - v' || cast(i.tipo_vinculo as varchar(10))
													else trim(cf.nome)
												end as nome,
												i.tipo_vinculo,
                                                pt.proposta_id,
                                                p.integrar_no_sga,
                                                p.data_realizacao_inicio
											from proposta_turma pt
                                            inner join proposta p on p.id = pt.proposta_id and not p.excluido
											inner join inscricao i on i.proposta_turma_id = pt.id and not i.excluido
											inner join usuario u on i.usuario_id = u.id and not u.excluido
											left join cargo_funcao cf on coalesce(i.funcao_id, i.cargo_id) = cf.id and not cf.excluido
											where
												not pt.excluido
												and pt.proposta_id = @propostaId ");
            if (!string.IsNullOrEmpty(login))
                query.AppendLine($" and u.login like '%{@login}%' ");
            if (!string.IsNullOrEmpty(cpf))
                query.AppendLine($"and u.cpf like '%{@cpf}%' ");
            if (!string.IsNullOrEmpty(nomeCursista))
                query.AppendLine($" and lower(u.nome) like '%{@nomeCursista.ToLower()}%' ");
            if (turmasId?.Length > 0)
                query.AppendLine($" and pt.id = any(@turmasId) ");

            query.AppendLine(" order by pt.nome, u.nome");
            query.AppendLine(" limit @numeroRegistros offset @registrosIgnorados ");

            var registrosIgnorados = numeroPagina > 1 ? (numeroPagina - 1) * numeroRegistros : 0;
            return conexao.Obter().QueryAsync<Inscricao, PropostaTurma, Usuario, CargoFuncao, Proposta, Inscricao>(query.ToString(), (inscricao, propostaTurma, usuario, cargoFuncao, proposta) =>
               {
                   propostaTurma.Proposta = proposta;
                   inscricao.PropostaTurma = propostaTurma;
                   inscricao.Funcao = cargoFuncao;
                   inscricao.Usuario = usuario;
                   return inscricao;
               }, new { propostaId, login, cpf, nomeCursista, turmasId, numeroRegistros, registrosIgnorados }, splitOn: "id, proposta_turma_id, usuario_id,cargo_id,proposta_id");
        }

        public Task<int> ObterInscricaoPorIdComFiltrosTotalRegistros(long propostaId, string? login, string? cpf, string? nomeCursista, long[]? turmasId)
        {
            var query = new StringBuilder(@"select count(1)
											from proposta_turma pt
											inner join inscricao i on i.proposta_turma_id = pt.id and not i.excluido
											inner join usuario u on i.usuario_id = u.id and not u.excluido
											left join cargo_funcao cf on coalesce(i.cargo_id, i.funcao_id) = cf.id and not cf.excluido
											where
                                                not pt.excluido
												and pt.proposta_id = @propostaId ");

            if (!string.IsNullOrEmpty(login))
                query.AppendLine($" and u.login like '%{@login}%' ");

            if (!string.IsNullOrEmpty(cpf))
                query.AppendLine($"and u.cpf like '%{@cpf}%' ");

            if (!string.IsNullOrEmpty(nomeCursista))
                query.AppendLine($" and lower(u.nome) like '%{@nomeCursista.ToLower()}%' ");

            if (turmasId?.Length > 0)
                query.AppendLine($" and pt.id = any(@turmasId) ");

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { propostaId, login, cpf, nomeCursista, turmasId });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, int numeroPagina, int numeroRegistros, long? numeroHomologacao)
        {
            var situacaoProposta = (int)SituacaoProposta.Publicada;
            var query = @"  
                    select 
               		    p.id,
               		    p.nome_formacao
                    from proposta p
                    where not p.excluido 
                      and p.situacao = @situacaoProposta  ";

            if (areaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
                query += " and p.area_promotora_id = @areaPromotoraIdUsuarioLogado";

            if (!string.IsNullOrEmpty(nomeFormacao))
                query += $" and lower(p.nome_formacao) like '%{nomeFormacao.ToLower()}%' ";

            if (codigoDaFormacao != null)
                query += "  and p.id = @codigoDaFormacao ";

            if (numeroHomologacao.NaoEhNulo())
                query += "  and p.numero_homologacao = @numeroHomologacao ";

            query += " order by p.id desc limit @numeroRegistros offset @registrosIgnorados  ";

            var registrosIgnorados = numeroPagina > 1 ? (numeroPagina - 1) * numeroRegistros : 0;
            var parametros = new { areaPromotoraIdUsuarioLogado, nomeFormacao, codigoDaFormacao, numeroRegistros, registrosIgnorados, situacaoProposta, numeroHomologacao };
            return conexao.Obter().QueryAsync<Proposta>(query.ToString(), parametros);
        }

        public Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, long? numeroHomologacao)
        {
            var situacaoProposta = (int)SituacaoProposta.Publicada;
            var query = @"	
                select count(1) 
                from proposta p
                where not p.excluido 
                  and p.situacao = @situacaoProposta ";

            if (areaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
                query += " and p.area_promotora_id = @areaPromotoraIdUsuarioLogado";

            if (!string.IsNullOrEmpty(nomeFormacao))
                query += $" and lower(p.nome_formacao) like '%{nomeFormacao.ToLower()}%' ";

            if (codigoDaFormacao != null)
                query += "  and p.id = @codigoDaFormacao ";

            if (numeroHomologacao.NaoEhNulo())
                query += " and p.numero_homologacao = @numeroHomologacao ";

            return conexao.Obter().ExecuteScalarAsync<int>(query, new { areaPromotoraIdUsuarioLogado, nomeFormacao, codigoDaFormacao, situacaoProposta, numeroHomologacao });
        }

        public Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds, long? propostaTurmaId = null)
        {
            var query = @" 
            WITH inscricoes_turma AS (
                SELECT
                    pt.id,
                    COUNT(*) AS total_inscricoes,
                    COUNT(*) FILTER (WHERE i.situacao = 1) AS confirmadas,
                    COUNT(*) FILTER (WHERE i.situacao = 3) AS aguardandoAnalise,
                    COUNT(*) FILTER (WHERE i.situacao = 5) AS emEspera,
                    COUNT(*) FILTER (WHERE i.situacao = 4) AS cancelada
                FROM proposta_turma pt
                INNER JOIN inscricao i ON i.proposta_turma_id = pt.id AND NOT i.excluido
                WHERE NOT pt.excluido
                  AND pt.proposta_id = ANY(@propostaIds)
                GROUP BY pt.id
            ), criterio_validacao_permite_sorteio AS (
	            SELECT 
		            pcvi.proposta_id
	            FROM proposta_criterio_validacao_inscricao pcvi 
	            LEFT JOIN criterio_validacao_inscricao cvi ON cvi.id = pcvi.criterio_validacao_inscricao_id AND NOT cvi.excluido 
	            WHERE NOT pcvi.excluido 
	              AND cvi.permite_sorteio 
	              AND pcvi.proposta_id = ANY(@propostaIds)
            )
            SELECT
                p.id AS propostaId,
                pt.id as propostaTurmaId,
                p.quantidade_vagas_turma AS quantidadeVagas,
                pt.nome AS nomeTurma,
                CASE
                    WHEN ped.data_fim IS NULL THEN TO_CHAR(ped.data_inicio, 'dd/mm/yyyy')
                    ELSE TO_CHAR(ped.data_inicio, 'dd/mm/yyyy') || ' até ' || TO_CHAR(ped.data_fim, 'dd/mm/yyyy')
                END AS datas,
                it.total_inscricoes AS totalInscricoes,
                it.confirmadas,
                it.aguardandoAnalise,
                it.emEspera,
                it.cancelada,
                p.quantidade_vagas_turma - it.Confirmadas as disponiveis,
                CASE 
                    WHEN it.total_inscricoes <= p.quantidade_vagas_turma THEN 0 
                    ELSE it.total_inscricoes - p.quantidade_vagas_turma
                END AS excedidas,
                CASE 
                    WHEN cvps.proposta_id IS NOT NULL THEN TRUE
                    ELSE FALSE
                END AS permiteSorteio
            FROM proposta p
            LEFT JOIN proposta_turma pt ON pt.proposta_id = p.id AND NOT pt.excluido
            LEFT JOIN proposta_encontro_turma pet ON pet.turma_id = pt.id AND NOT pet.excluido
            LEFT JOIN proposta_encontro pe ON pe.id = pet.proposta_encontro_id AND NOT pe.excluido
            LEFT JOIN proposta_encontro_data ped ON ped.proposta_encontro_id = pe.id AND NOT ped.excluido
            LEFT JOIN inscricoes_turma it ON it.id = pt.id
            LEFT JOIN criterio_validacao_permite_sorteio cvps on cvps.proposta_id = p.id
            WHERE NOT p.excluido
              AND p.id = ANY(@propostaIds)";

            if (propostaTurmaId.HasValue)
                query += " AND pt.id = @propostaTurmaId ";

            query += " ORDER BY pt.nome ";

            return conexao.Obter().QueryAsync<ListagemFormacaoComTurmaDTO>(query, new { propostaIds, propostaTurmaId });
        }

        public Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorPropostaIds(long[] codigosFormacao)
        {
            var query = @"select id, proposta_id, tipo_inscricao from proposta_tipo_inscricao where proposta_id = any(@codigosFormacao) and not excluido";

            return conexao.Obter().QueryAsync<PropostaTipoInscricao>(query.ToString(), new { codigosFormacao });
        }

        public async Task<IEnumerable<Inscricao>> ObterInscricoesConfirmadas()
        {
            const string query = @"select i.id,
                                        i.proposta_turma_id,
                                        i.usuario_id,
                                        i.cargo_codigo,
                                        i.cargo_dre_codigo,
                                        i.cargo_ue_codigo,
                                        i.cargo_id,
                                        i.funcao_codigo,
                                        i.funcao_dre_codigo,
                                        i.funcao_ue_codigo,
                                        i.funcao_id,
                                        u.login,
                                        u.cpf
                                    from inscricao i
                                    inner join usuario u on i.usuario_id = u.id and not u.excluido
                                    where not i.excluido
                                    and i.situacao = @situacao";

            return await conexao.Obter().QueryAsync<Inscricao, Usuario, Inscricao>(query, (inscricao, usuario) =>
               {
                   inscricao.Usuario = usuario;
                   return inscricao;
               }, new { situacao = (int)SituacaoInscricao.Confirmada }, splitOn: "id, login");
        }

        public async Task<IEnumerable<InscricaoUsuarioInternoDto>> ObterInscricoesPorPropostasTurmasIdUsuariosInternos(long[] propostasTurmasId)
        {
            var tipoUsuario = (int)TipoUsuario.Interno;
            var query = @$"
                            select i.id as InscricaoId ,u.id as UsuarioId, u.login from inscricao i 
                            inner join usuario u on i.usuario_id = u.id 
                            where not i.excluido  and u.tipo = @tipoUsuario
                            and i.proposta_turma_id = any(@propostasTurmasId)
                        ";
            return await conexao.Obter().QueryAsync<InscricaoUsuarioInternoDto>(query, new { propostasTurmasId, tipoUsuario });
        }
        
        public async Task<IEnumerable<InscricaoPossuiAnexoDTO>> ObterSeInscricaoPossuiAnexoPorPropostasIds(long[] inscricoesId)
        {
            var query = @$"
                        select 
	                            i.id as InscricaoId,
	                            a.nome NomeArquivo,
	                            a.codigo 
                            from 
	                            proposta_turma pt 
	                            	left join inscricao i on pt.id = i.proposta_turma_id 
	                                left join arquivo a on i.arquivo_id = a.id 
                            where i.id = any(@inscricoesId) 
                         ";
            return await conexao.Obter().QueryAsync<InscricaoPossuiAnexoDTO>(query, new { inscricoesId });
        }
        
        public async Task<IEnumerable<InscricaoDadosEmailConfirmacao>> ObterDadasInscricaoPorInscricaoId(long inscricoeId)
        {
            var query = @$" select 
	                            i.usuario_id UsuarioId,
	                            u.email ,
	                            u.nome NomeDestinatario,
	                            ped.data_inicio DataInicio,
	                            ped.data_fim DataFim,
	                            pe.hora_inicio HoraInicio,
	                            pe.hora_fim HoraFim,
	                            pe.local,
	                            p.integrar_no_sga IntegradoSga,
	                            p.id ||' - ' || p.nome_formacao nomeFormacao
                            from 
	                            inscricao i 
	                            inner join usuario u on i.usuario_id = u.id 
	                            inner join proposta_turma pt on i.proposta_turma_id = pt.id
	                            inner join proposta p on pt.proposta_id = p.id 
	                            inner join proposta_encontro pe on pe.proposta_id  = p.id
	                            inner join proposta_encontro_data ped on ped.proposta_encontro_id  = pe.id 
                            where i.id = @inscricoeId ;
                        ";
            return await conexao.Obter().QueryAsync<InscricaoDadosEmailConfirmacao>(query, new { inscricoeId });
        }

        public Task<IEnumerable<long>> ObterIdsInscricoesAguardandoAnalise(long propostaTurmaId)
        {
            var situacao = SituacaoInscricao.AguardandoAnalise;
            var query = @"select id from inscricao where not excluido and situacao = @situacao and proposta_turma_id = @propostaTurmaId";
            return conexao.Obter().QueryAsync<long>(query, new { situacao, propostaTurmaId });
        }
    }
}
