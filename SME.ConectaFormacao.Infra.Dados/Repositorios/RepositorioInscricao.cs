using System.Text;
using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioInscricao : RepositorioBaseAuditavel<Inscricao>, IRepositorioInscricao
    {
        private readonly int QUANTIDADE_MINIMA_PARA_PAGINAR = 10;
        public RepositorioInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public async Task<bool> ConfirmarInscricaoVaga(Inscricao inscricao)
        {
            PreencherAuditoriaAlteracao(inscricao);

            var query = @"update proposta_turma_vaga set  
                            inscricao_id = @Id,
                            alterado_em = @AlteradoEm, 
                            alterado_por = @AlteradoPor, 
                            alterado_login = @AlteradoLogin
                          where id = (
                                select id 
                                from proposta_turma_vaga 
                                where proposta_turma_id = @PropostaTurmaId 
                                  and inscricao_id is null limit 1)";

            return await conexao.Obter().ExecuteAsync(query, inscricao) > 0;
        }

        public Task<bool> ExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var situacaoCancelada = (int)SituacaoInscricao.Cancelada;

            var query = @"select 1 
                          from inscricao i 
                          inner join proposta_turma pt on pt.id = i.proposta_turma_id and not pt.excluido
                          where i.usuario_id = @usuarioId 
                            and pt.proposta_id = @propostaId
                            and i.situacao <> @situacaoCancelada
                            and not i.excluido
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
		                when i.cargo_id is not null
		                and i.funcao_id is not null then concat(cfc.nome,
		                '/',
		                cff.nome)
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

        public Task<IEnumerable<Inscricao>> ObterInscricaoPorIdComFiltros(long inscricaoId, string? login, string? cpf, string? nomeCursista, string? nomeTurma,
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
												cf.nome
											from
												inscricao i
											inner join proposta_turma pt on
												i.proposta_turma_id = pt.id
											inner join usuario u on
												i.usuario_id = u.id
											inner join cargo_funcao cf on
												coalesce(i.cargo_id,i.funcao_id) = cf.id
											where
												not i.excluido
												and not pt.excluido
												and not u.excluido
												and not cf.excluido
												and pt.proposta_id = @inscricaoId ");
            if (!string.IsNullOrEmpty(login))
                query.AppendLine($" and u.login like '%{@login}%' ");
            if (!string.IsNullOrEmpty(cpf))
                query.AppendLine($"and u.cpf like '%{@cpf}%' ");
            if (!string.IsNullOrEmpty(nomeCursista))
                query.AppendLine($" and lower(u.nome) like '%{@nomeCursista.ToLower()}%' ");
            if (!string.IsNullOrEmpty(nomeTurma))
                query.AppendLine($" and lower(pt.nome) like '%{nomeTurma.ToLower()}%' ");
            query.AppendLine(" limit @numeroRegistros offset @registrosIgnorados ");
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;
            return conexao.Obter().QueryAsync<Inscricao, PropostaTurma, Usuario, CargoFuncao, Inscricao>(query.ToString(), (inscricao, propostaTurma, usuario, cargoFuncao) =>
               {
                   inscricao.PropostaTurma = propostaTurma;
                   inscricao.Funcao = cargoFuncao;
                   inscricao.Usuario = usuario;
                   return inscricao;
               }, new { inscricaoId, login, cpf, nomeCursista, numeroRegistros, registrosIgnorados }, splitOn: "id, proposta_turma_id, usuario_id,cargo_id");
        }

        public Task<int> ObterInscricaoPorIdComFiltrosTotalRegistros(long inscricaoId, string? login, string? cpf, string? nomeCursista, string? nomeTurma)
        {
            var query = new StringBuilder(@"select count(1) from (select *
											from
												inscricao i
											inner join proposta_turma pt on
												i.proposta_turma_id = pt.id
											inner join usuario u on
												i.usuario_id = u.id
											inner join cargo_funcao cf on
												coalesce(i.cargo_id,i.funcao_id) = cf.id
											where
												not i.excluido
												and not pt.excluido
												and not u.excluido
												and not cf.excluido
												and pt.proposta_id = @inscricaoId ");
            if (!string.IsNullOrEmpty(login))
                query.AppendLine($" and u.login like '%{@login}%' ");
            if (!string.IsNullOrEmpty(cpf))
                query.AppendLine($"and u.cpf like '%{@cpf}%' ");
            if (!string.IsNullOrEmpty(nomeCursista))
                query.AppendLine($" and lower(u.nome) like '%{@nomeCursista.ToLower()}%' ");
            if (!string.IsNullOrEmpty(nomeTurma))
                query.AppendLine($" and lower(pt.nome) like '%{nomeTurma.ToLower()}%' ");
            query.AppendLine(" )tb ");

            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { inscricaoId, login, cpf, nomeCursista });
        }

        public Task<IEnumerable<Proposta>> ObterDadosPaginadosComFiltros(long? codigoDaFormacao,
            string? nomeFormacao, int numeroPagina, int numeroRegistros, int totalRegistrosFiltro)
        {
            var situacaoProposta = (int)SituacaoProposta.Publicada;
            var sql = new StringBuilder(@"  select * from(select 
               		pt.proposta_id as id,
               		p.nome_formacao
               from proposta_turma pt
               inner join proposta p on p.id = pt.proposta_id
               left join inscricao i on i.proposta_turma_id = pt.id
               where not p.excluido and not pt.excluido  
                and p.situacao = @situacaoProposta  ");

            if (codigoDaFormacao != null)
                sql.AppendLine(@" and pt.proposta_id = @codigoDaFormacao ");
            if (!string.IsNullOrEmpty(nomeFormacao))
                sql.AppendLine($" and lower(p.nome_formacao) like '%{nomeFormacao.ToLower()}%' ");

            sql.AppendLine(@"  group by pt.proposta_id,p.nome_formacao)insc 
                                      order by id desc
                                     limit @numeroRegistros offset @registrosIgnorados  ");


            var registrosIgnorados = totalRegistrosFiltro - numeroRegistros >= QUANTIDADE_MINIMA_PARA_PAGINAR ? (numeroPagina - 1) * numeroRegistros : 0;
            var parametros = new { nomeFormacao, codigoDaFormacao, numeroRegistros, registrosIgnorados, situacaoProposta };
            return conexao.Obter().QueryAsync<Proposta>(sql.ToString(), parametros);
        }
        public Task<int> ObterDadosPaginadosComFiltrosTotalRegistros(long? codigoDaFormacao,
                string? nomeFormacao)
        {
            var situacaoProposta = (int)SituacaoProposta.Publicada;
            var query = new StringBuilder(@"	select count(1) from(select 
               		pt.proposta_id as id,
               		p.nome_formacao
               from proposta_turma pt
               inner join proposta p on p.id = pt.proposta_id
               left join inscricao i on i.proposta_turma_id = pt.id
               where not p.excluido and not pt.excluido  
                and p.situacao = @situacaoProposta  ");
            if (!string.IsNullOrEmpty(nomeFormacao))
                query.AppendLine($" and lower(p.nome_formacao) like '%{nomeFormacao.ToLower()}%'  ");
            if (codigoDaFormacao != null)
                query.AppendLine("  and pt.proposta_id = @codigoDaFormacao ");

            query.AppendLine(@" group by pt.proposta_id,p.nome_formacao)insc  ");
            return conexao.Obter().ExecuteScalarAsync<int>(query.ToString(), new { nomeFormacao, codigoDaFormacao, situacaoProposta });
        }

        public Task<IEnumerable<ListagemFormacaoComTurmaDTO>> DadosListagemFormacaoComTurma(long[] propostaIds)
        {
            var query = new StringBuilder(@" select * from(select  distinct 
                                                	count(i.id) totalInscricoes,
												    pt.nome NomeTurma,
												    pt.proposta_id PropostaId,
												    case 
												    	when ped.data_fim is null
												    	then TO_CHAR(ped.data_inicio,'dd/mm/yyyy')
												    	else  TO_CHAR(ped.data_inicio,'dd/mm/yyyy')||' até '||TO_CHAR(ped.data_fim,'dd/mm/yyyy')
												    end as Datas,
													p.quantidade_vagas_turma QuantidadeVagas ,
                                                    ped.data_inicio
												from proposta p 
												inner join proposta_encontro pe on p.id = pe.proposta_id 
												inner join proposta_encontro_turma pet on pe.id = pet.proposta_encontro_id 
												inner join proposta_turma pt on p.id = pt.proposta_id  and pt.id = pet.turma_id 
												inner join proposta_encontro_data ped on pe.id = ped.proposta_encontro_id 
												left join inscricao i  on i.proposta_turma_id = pt.id
												where p.id = any(@propostaIds) 
												group by pt.nome,pt.proposta_id,ped.data_fim,ped.data_inicio,p.quantidade_vagas_turma
											     ) as consulta
										        order by data_inicio ");

            return conexao.Obter().QueryAsync<ListagemFormacaoComTurmaDTO>
                (query.ToString(), new { propostaIds });
        }
    }
}
