using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Parametros
{
    [ExcludeFromCodeCoverage]
    public class DependenciasNotificacao(
        ITransacao transacao,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario) : IDependenciasNotificacao
    {
        public ITransacao Transacao { get; } = transacao;
        public IRepositorioNotificacao RepositorioNotificacao { get; } = repositorioNotificacao;
        public IRepositorioNotificacaoUsuario RepositorioNotificacaoUsuario { get; } = repositorioNotificacaoUsuario;
    }
}
