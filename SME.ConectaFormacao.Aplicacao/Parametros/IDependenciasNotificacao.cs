using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Parametros
{
    public interface IDependenciasNotificacao
    {
        ITransacao Transacao { get; }
        IRepositorioNotificacao RepositorioNotificacao { get; }
        IRepositorioNotificacaoUsuario RepositorioNotificacaoUsuario { get; }
    }
}
