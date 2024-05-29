using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioNotificacao : IRepositorioBaseAuditavel<Notificacao>
    {
        Task<IEnumerable<Notificacao>> ObterNotificacaoPaginada(string login, long? id, string? titulo, NotificacaoCategoria? categoria, NotificacaoTipo? tipo, NotificacaoUsuarioSituacao? situacao, int numeroRegistros, int quantidadeRegistrosIgnorados);
        Task<long> ObterTotalNaoLidoPorUsuario(string login);
        Task<int> ObterTotalNotificacao(string login, long? id, string? titulo, NotificacaoCategoria? categoria, NotificacaoTipo? tipo, NotificacaoUsuarioSituacao? situacao);
    }
}
