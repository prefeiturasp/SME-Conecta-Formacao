using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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

        public Task<string> ObterCargoFuncaoPorId(long id)
        {
            var query = @"
                select
	                case
		                when i.funcao_id is not null then cff.nome
		                when i.cargo_id is not null then cfc.nome
		                else ''
	                end as cargo_funcao
                from
	                inscricao i
                left join cargo_funcao cfc on
	                cfc.id = i.cargo_id
                left join cargo_funcao cff on
	                cff.id = i.funcao_id
                where
	                i.id = @id
                ";

            return conexao.Obter().ExecuteScalarAsync<string>(query, new { id });
        }

        public Task<IEnumerable<Inscricao>> ObterDadosPaginadosPorUsuarioId(long usuarioId, int numeroPagina, int numeroRegistros)
        {
            var query = @"
                select i.id,
                       i.situacao,
                       i.proposta_turma_id,
                       pt.nome,
                       pt.proposta_id,
                       p.nome_formacao,
                       p.data_realizacao_inicio,
                       p.data_realizacao_fim,
                       p.id as Id 
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

        public Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long propostaId, string? login, string? cpf, string? nomeCursista, double? turmaId,
            int numeroPagina, int numeroRegistros)
        {
            var query = new StringBuilder(@"select 
                                                i.id,
												i.situacao,
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
												i.tipo_vinculo
											from proposta_turma pt
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
            if (turmaId.HasValue)
                query.AppendLine($" and pt.id = @turmaId ");

            query.AppendLine(" order by pt.nome, u.nome");
            query.AppendLine(" limit @numeroRegistros offset @registrosIgnorados ");

            var registrosIgnorados = numeroPagina > 1 ? (numeroPagina - 1) * numeroRegistros : 0;
            return conexao.Obter().QueryAsync<Inscricao, PropostaTurma, Usuario, CargoFuncao, Inscricao>(query.ToString(), (inscricao, propostaTurma, usuario, cargoFuncao) =>
               {
                   inscricao.PropostaTurma = propostaTurma;
                   inscricao.Funcao = cargoFuncao;
                   inscricao.Usuario = usuario;
                   return inscricao;
               }, new { propostaId, login, cpf, nomeCursista, turmaId, numeroRegistros, registrosIgnorados }, splitOn: "id, proposta_turma_id, usuario_id,cargo_id");
        }

        public Task<int> ObterInscricaoPorIdComFiltrosTotalRegistros(long propostaId, string? login, string? cpf, string? nomeCursista, double? turmaId)
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

            if (turmaId.HasValue)
                query.AppendLine($" and pt.id = @turmaId ");

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { propostaId, login, cpf, nomeCursista, turmaId });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao, int numeroPagina, int numeroRegistros)
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

            query += " order by p.id desc limit @numeroRegistros offset @registrosIgnorados  ";

            var registrosIgnorados = numeroPagina > 1 ? (numeroPagina - 1) * numeroRegistros : 0;
            var parametros = new { areaPromotoraIdUsuarioLogado, nomeFormacao, codigoDaFormacao, numeroRegistros, registrosIgnorados, situacaoProposta };
            return conexao.Obter().QueryAsync<Proposta>(query.ToString(), parametros);
        }

        public Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? areaPromotoraIdUsuarioLogado, long? codigoDaFormacao, string? nomeFormacao)
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

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { areaPromotoraIdUsuarioLogado, nomeFormacao, codigoDaFormacao, situacaoProposta });
        }

        public Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds)
        {
            var query = @" 
                    with inscricoes_turma as (
                        select pt.id, count(1) as total_inscricoes
                        from proposta_turma pt 
                        inner join inscricao i on i.proposta_turma_id = pt.id and not i.excluido 
                        where not pt.excluido 
                          and i.situacao = 1
                          and pt.proposta_id = any(@propostaIds)
                        group by pt.id
                    )

                    select
                        p.id as PropostaId,
                        p.quantidade_vagas_turma as QuantidadeVagas,
                        pt.nome as NomeTurma,
                        case 
                         when ped.data_fim is null then TO_CHAR(ped.data_inicio, 'dd/mm/yyyy')
                         else TO_CHAR(ped.data_inicio, 'dd/mm/yyyy')|| ' até ' || TO_CHAR(ped.data_fim, 'dd/mm/yyyy')
                        end as Datas,
                        it.total_inscricoes as totalinscricoes
                    from proposta p
                    left join proposta_turma pt on pt.proposta_id = p.id and not pt.excluido
                    left join proposta_encontro_turma pet on pet.turma_id = pt.id and not pet.excluido
                    left join proposta_encontro pe on pe.id = pet.proposta_encontro_id and not pe.excluido
                    left join proposta_encontro_data ped on ped.proposta_encontro_id = pe.id and not ped.excluido
                    left join inscricoes_turma it on it.id = pt.id
                    where not p.excluido and p.id = any(@propostaIds)
                    order by pt.nome ";

            return conexao.Obter().QueryAsync<ListagemFormacaoComTurmaDTO>(query.ToString(), new { propostaIds });
        }

        public Task<IEnumerable<PropostaTipoInscricao>> ObterTiposInscricaoPorPropostaIds(long[] codigosFormacao)
        {
            var query = @"select id, proposta_id, tipo_inscricao from proposta_tipo_inscricao where proposta_id = any(@codigosFormacao) and not excluido";

            return conexao.Obter().QueryAsync<PropostaTipoInscricao>(query.ToString(), new { codigosFormacao });
        }
    }
}
