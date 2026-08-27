using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Parametros
{
    public class DependenciasNotificacao : IDependenciasNotificacao
    {
        public ITransacao Transacao { get; }
        public IRepositorioNotificacao RepositorioNotificacao { get; }
        public IRepositorioNotificacaoUsuario RepositorioNotificacaoUsuario { get; }

        public DependenciasNotificacao(
            ITransacao transacao,
            IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario)
        {
            Transacao = transacao;
            RepositorioNotificacao = repositorioNotificacao;
            RepositorioNotificacaoUsuario = repositorioNotificacaoUsuario;
        }
    }
}
