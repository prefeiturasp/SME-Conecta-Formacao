using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarNotificacaoLidaCommand : IRequest<bool>
    {
        public EnviarNotificacaoLidaCommand(NotificacaoSignalRDTO notificacao)
        {
            Notificacao = notificacao;
        }

        public NotificacaoSignalRDTO Notificacao { get; }
    }

    public class EnviarNotificacaoLidaCommandValidator : AbstractValidator<EnviarNotificacaoLidaCommand>
    {
        public EnviarNotificacaoLidaCommandValidator()
        {
            RuleFor(c => c.Notificacao)
                .NotNull()
                .WithMessage("A notificação deve ser informada para o envio via signalR.");

            RuleFor(c => c.Notificacao.Id)
                .NotEmpty()
                .WithMessage("O identificador da notificação deve ser informado para o envio via signalR.");
            
            RuleFor(c => c.Notificacao.Titulo)
                .NotEmpty()
                .WithMessage("O título da mensagem deve ser informada para o envio via signalR.");

            RuleFor(c => c.Notificacao.Usuarios)
                .NotNull()
                .WithMessage("Os usuários devem ser informados para o envio via signalR.");
        }
    }
}