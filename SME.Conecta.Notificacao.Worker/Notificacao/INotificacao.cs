using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.Conecta.Notificacao.Worker.Notificacao
{
    public interface INotificacao
    {
        public string PrefixoUsuario { get; }
        public string PrefixoGrupo { get; }

        public Task NotificacaoCriadaUsuarios(MensagemRabbit mensagemRabbit);
        public Task NotificacaoCriadaGrupos(MensagemRabbit mensagemRabbit);

        public Task NotificacaoLidaUsuarios(MensagemRabbit mensagemRabbit);
        public Task NotificacaoLidaGrupos(MensagemRabbit mensagemRabbit);

        public Task NotificacaoExcluidaUsuarios(MensagemRabbit mensagemRabbit);
        public Task NotificacaoExcluidaGrupos(MensagemRabbit mensagemRabbit);
    }
}
