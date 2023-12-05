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

        public Task<IEnumerable<ComponenteCurricular>> ObterComponentesCurricularesPorModalidadeAnoLetivoAno(Modalidade modalidade, int anoLetivo, long anoId)
        {
            var query = $@"select cc.id, 
                                 cc.ano_id AnoId,
                                 cc.codigo_eol CodigoEOL,
                                 cc.nome,
                                 cc.todos,
                                 cc.ordem 
                          from componente_curricular cc
                            join ano a on a.id = cc.ano_id
                          where not cc.excluido
                              {IncluirFiltroPorModalidade(modalidade)}                              
                              {IncluirFiltroPorAno(anoId)}                              
                              and a.ano_letivo = @anoLetivo 
                              order by cc.ordem ";

            return conexao.Obter().QueryAsync<ComponenteCurricular>(query, new { modalidade, anoLetivo, anoId });
        }

        private string IncluirFiltroPorAno(long anoId)
        {
            return anoId == 999 ? string.Empty : " and cc.ano_id = @anoId ";
        }

        private string IncluirFiltroPorModalidade(Modalidade modalidade)
        {
            return modalidade == Modalidade.TODAS ? string.Empty : " and a.modalidade = @modalidade ";
        }
    }
}