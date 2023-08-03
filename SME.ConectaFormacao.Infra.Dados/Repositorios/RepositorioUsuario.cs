using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioUsuario : RepositorioBaseAuditavel<Usuario>, IRepositorioUsuario
    {
        public RepositorioUsuario(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        {
        }
    }
}
