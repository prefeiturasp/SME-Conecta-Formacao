using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioEmailEnviado : IRepositorioBaseAuditavel<EmailEnviado>
    {
        Task<bool> ExistePorChaveIdempotenciaAsync(string chaveIdempotencia);
        Task<EmailEnviado?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia);
        Task<IEnumerable<EmailEnviado>> ObterPorEmailDestinatarioAsync(string emailDestinatario);
        Task<IEnumerable<EmailEnviado>> ObterPorNotificacaoUsuarioIdAsync(long notificacaoUsuarioId);
    }
}
