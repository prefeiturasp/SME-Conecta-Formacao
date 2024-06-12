using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoQuery : IRequest<NotificacaoDTO>
    {
        public ObterNotificacaoQuery(long id, string login)
        {
            Id = id;
            Login = login;
        }

        public long Id { get; }
        public string Login { get; }
    }

    public class ObterNotificacaoQueryValidator : AbstractValidator<ObterNotificacaoQuery>
    {
        public ObterNotificacaoQueryValidator()
        {
            RuleFor(f => f.Id)
                .NotEmpty()
                .WithMessage("Informe o Id para obter a notificação");

            RuleFor(f => f.Login)
                .NotEmpty()
                .WithMessage("Informe o Login para obter a notificação");
        }
    }
}
