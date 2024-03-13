using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioAnoTurma : RepositorioBaseAuditavel<AnoTurma>, IRepositorioAnoTurma
    {
        public RepositorioAnoTurma(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }

        public Task<IEnumerable<AnoTurma>> ObterAnosPorModalidadeAnoLetivo(Modalidade[] modalidade, int anoLetivo, bool exibirTodos)
        {
            var modalidades = modalidade.Select(t => (long)t).ToArray();

            var query = $@"select id, 
                                 codigo_eol,
                                 descricao,
                                 codigo_serie_ensino,
                                 ano_letivo,
                                 modalidade,
                                 todos,
                                 ordem 
                          from ano_turma 
                          where not excluido
                              and (modalidade = any(@modalidades) or modalidade is null)
                              and ano_letivo = @anoLetivo ";

            if (!exibirTodos)
                query += " and not todos ";

            query += " order by ordem, descricao ";

            return conexao.Obter().QueryAsync<AnoTurma>(query, new { modalidades, anoLetivo });
        }

        public Task<IEnumerable<AnoTurma>> ObterPorAnoLetivo(int anoLetivo)
        {
            var query = $@"select id, 
                                 codigo_eol CodigoEOL,
                                 descricao,
                                 codigo_serie_ensino CodigoSerieEnsino,
                                 ano_letivo AnoLetivo,
                                 modalidade Modalidade,
                                 todos,
                                 ordem,
                                 criado_em,
                                 criado_por,
                                 alterado_em,
                                 alterado_por,
                                 criado_login,
                                 alterado_login 
                          from ano_turma 
                          where not excluido
                              and ano_letivo = @anoLetivo 
                              order by ordem ";

            return conexao.Obter().QueryAsync<AnoTurma>(query, new { anoLetivo });
        }
    }
}