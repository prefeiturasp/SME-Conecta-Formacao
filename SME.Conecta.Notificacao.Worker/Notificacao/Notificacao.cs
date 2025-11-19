using Microsoft.AspNetCore.SignalR;
using SME.Conecta.Notificacao.Worker.Hub;
using SME.Conecta.Notificacao.Worker.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.Conecta.Notificacao.Worker.Notificacao
{
    public class Notificacao : INotificacao
    {
        public string PrefixoUsuario => "usuario-";
        public string PrefixoGrupo => "grupo-";

        private readonly IHubContext<NotificacaoHub> _hubContext;

        public Notificacao(IHubContext<NotificacaoHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        #region Enviar Mensagem

        private Task EnviarMensagem(string cliente, string metodo, MensagemNotificacaoDTO mensagemNotificacaoDTO)
        {
            return _hubContext.Clients.Group(cliente).SendAsync(metodo, mensagemNotificacaoDTO);
        }

        private async Task EnviarMensagemUsuarios(string metodo, MensagemNotificacaoUsuariosDTO mensagemNotificacaoUsuariosDTO)
        {
            foreach (var usuario in mensagemNotificacaoUsuariosDTO.Usuarios)
            {
                await EnviarMensagem(string.Concat(PrefixoUsuario, usuario), metodo, mensagemNotificacaoUsuariosDTO);
            }
        }

        private async Task EnviarMensagemGrupos(string metodo, MensagemNotificacaoGruposDTO mensagemNotificacaoGruposDTO)
        {
            foreach (var grupo in mensagemNotificacaoGruposDTO.Grupos)
            {
                await EnviarMensagem(string.Concat(PrefixoGrupo, grupo), metodo, mensagemNotificacaoGruposDTO);
            }
        }

        #endregion

        public async Task NotificacaoCriadaUsuarios(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoCriadaUsuariosDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoUsuariosDTO>();
            await EnviarMensagemUsuarios(NotificacaoEventos.Criada, mensagemNotificacaoCriadaUsuariosDTO);
        }

        public async Task NotificacaoCriadaGrupos(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoGruposDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoGruposDTO>();
            await EnviarMensagemGrupos(NotificacaoEventos.Criada, mensagemNotificacaoGruposDTO);
        }

        public async Task NotificacaoLidaUsuarios(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoCriadaUsuariosDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoUsuariosDTO>();
            await EnviarMensagemUsuarios(NotificacaoEventos.Lida, mensagemNotificacaoCriadaUsuariosDTO);
        }

        public async Task NotificacaoLidaGrupos(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoGruposDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoGruposDTO>();
            await EnviarMensagemGrupos(NotificacaoEventos.Lida, mensagemNotificacaoGruposDTO);
        }

        public async Task NotificacaoExcluidaUsuarios(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoCriadaUsuariosDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoUsuariosDTO>();
            await EnviarMensagemUsuarios(NotificacaoEventos.Excluida, mensagemNotificacaoCriadaUsuariosDTO);
        }

        public async Task NotificacaoExcluidaGrupos(MensagemRabbit mensagemRabbit)
        {
            var mensagemNotificacaoGruposDTO = mensagemRabbit.ObterObjetoMensagem<MensagemNotificacaoGruposDTO>();
            await EnviarMensagemGrupos(NotificacaoEventos.Excluida, mensagemNotificacaoGruposDTO);
        }
    }
}
