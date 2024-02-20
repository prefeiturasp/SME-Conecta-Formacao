using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioComponenteCurricular : RepositorioBaseAuditavel<ComponenteCurricular>, IRepositorioComponenteCurricular
    {
        public RepositorioComponenteCurricular(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorAnoTurma(long[] anoTurmaId, bool exibirOpcaoTodos)
        {
            var query = $@"select 
                               cc.id, 
                               cc.ano_turma_id,
                               cc.codigo_eol,
                               cc.nome,
                               cc.todos,
                               cc.ordem 
                           from componente_curricular cc
                           where not cc.excluido 
                           and (exists (
	                            select 1 
	                            from ano_turma a 
	                            where not a.excluido 
  	                              and a.id = cc.ano_turma_id 
	                              and a.id = any(@anoTurmaId)
	                              and not a.todos
                           ) or exists (
	                            select 1 
	                            from ano_turma a 
	                            where not a.excluido
	                              and a.id = any(@anoTurmaId)
	                              and a.todos  
                           ) or cc.ano_turma_id is null)";

            if (!exibirOpcaoTodos)
                query += " and not todos";

            query += " order by ordem";

            return conexao.Obter().QueryAsync<ComponenteCurricular>(query, new { anoTurmaId });
        }

        public Task<IEnumerable<ComponenteCurricular>> ObterPorAnoLetivo(int anoLetivo)
        {
            var query = $@"select cc.id, 
                                 cc.ano_turma_id AnoTurmaId,
                                 cc.codigo_eol CodigoEOL,
                                 cc.nome,
                                 cc.todos,
                                 cc.ordem,
                                 cc.criado_em,
                                 cc.criado_por,
                                 cc.alterado_em,
                                 cc.alterado_por,
                                 cc.criado_login,
                                 cc.alterado_login 
                          from componente_curricular cc
                            join ano_turma a on a.id = cc.ano_turma_id
                          where not cc.excluido
                              and a.ano_letivo = @anoLetivo 
                              order by cc.ordem ";

            return conexao.Obter().QueryAsync<ComponenteCurricular>(query, new { anoLetivo});
        }
    }
}