using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioComponenteCurricular : RepositorioBaseAuditavel<ComponenteCurricular>, IRepositorioComponenteCurricular
    {
        public RepositorioComponenteCurricular(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
    }
}