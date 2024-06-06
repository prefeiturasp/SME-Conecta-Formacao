using SME.Conecta.Notificacao.Worker.Notificacao;

namespace SME.Conecta.Notificacao.Worker.Hub
{
    public class NotificacaoHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly INotificacao _notificacao;

        public NotificacaoHub(INotificacao notificacao)
        {
            _notificacao = notificacao ?? throw new ArgumentNullException(nameof(notificacao));
        }

        public async Task Registrar(string usuario, string grupo)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, string.Concat(_notificacao.PrefixoGrupo, grupo));
            await Groups.AddToGroupAsync(Context.ConnectionId, string.Concat(_notificacao.PrefixoUsuario, usuario));
        }
    }
}
