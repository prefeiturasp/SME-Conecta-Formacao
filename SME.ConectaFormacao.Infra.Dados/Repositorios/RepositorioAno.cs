using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioAno : RepositorioBaseAuditavel<Ano>, IRepositorioAno
    {
        public RepositorioAno(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<IEnumerable<Ano>> ObterAnosPorModalidadeAnoLetivo(Modalidade modalidade, int anoLetivo)
        {
            var query = $@"select id, 
                                 codigo_eol CodigoEOL,
                                 descricao,
                                 codigo_serie_ensino CodigoSerieEnsino,
                                 ano_letivo AnoLetivo,
                                 modalidade Modalidade,
                                 todos,
                                 ordem 
                          from ano 
                          where not excluido
                              {IncluirFiltroPorModalidade(modalidade)}                              
                              and ano_letivo = @anoLetivo 
                              order by ordem ";

            return conexao.Obter().QueryAsync<Ano>(query, new { modalidade, anoLetivo });
        }

        private string IncluirFiltroPorModalidade(Modalidade modalidade)
        {
            return modalidade == Modalidade.TODAS ? string.Empty : " and modalidade = @modalidade ";
        }
    }
}