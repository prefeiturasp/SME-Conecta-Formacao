using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

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
    }
}
