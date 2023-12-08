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

        public Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorModalidadeAnoLetivoAno(Modalidade modalidade, int anoLetivo, long anoTurmaId)
        {
            var query = $@"select cc.id, 
                                 cc.ano_turma_id AnoTurmaId,
                                 cc.codigo_eol CodigoEOL,
                                 cc.nome,
                                 cc.todos,
                                 cc.ordem 
                          from componente_curricular cc
                            join ano_turma a on a.id = cc.ano_turma_id
                          where not cc.excluido
                              {IncluirFiltroPorModalidade(modalidade)}                              
                              {IncluirFiltroPorAno(anoTurmaId)}                              
                              and a.ano_letivo = @anoLetivo 
                              order by cc.ordem ";

            return conexao.Obter().QueryAsync<ComponenteCurricular>(query, new { modalidade, anoLetivo, anoTurmaId });
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

        private string IncluirFiltroPorAno(long anoTurmaId)
        {
            return anoTurmaId == 999 ? string.Empty : " and cc.ano_turma_id = @anoTurmaId ";
        }

        private string IncluirFiltroPorModalidade(Modalidade modalidade)
        {
            return modalidade == Modalidade.TODAS ? string.Empty : " and a.modalidade = @modalidade ";
        }
    }
}