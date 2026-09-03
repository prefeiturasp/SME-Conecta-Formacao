using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Servicos
{
    public interface IServicoNotificacao
    {
        Task<bool> PersistirEEnviarAsync(Notificacao notificacao, CancellationToken cancellationToken = default);
    }
}
