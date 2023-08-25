using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioCriterioAvaliacaoInscricao : RepositorioBaseAuditavel<CriterioValidacaoInscricao>, IRepositorioCriterioAvaliacaoInscricao
    {
        public RepositorioCriterioAvaliacaoInscricao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
    }
}
