using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioNotificacaoUsuario : IRepositorioBaseAuditavel<NotificacaoUsuario>
    {
        Task<NotificacaoUsuario> ObterNotificacaoUsuario(long id, string login);
        Task InserirUsuarios(IDbTransaction transacao, IEnumerable<NotificacaoUsuario> usuarios, long notificacaoId);
    }
}
