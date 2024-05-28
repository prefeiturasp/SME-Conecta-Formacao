using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalNotificacaoNaoLidaPorUsuarioQuery : IRequest<long>
    {
        public ObterTotalNotificacaoNaoLidaPorUsuarioQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterTotalNotificacaoNaoLidaPorUsuarioQueryValidator : AbstractValidator<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>
    {
        public ObterTotalNotificacaoNaoLidaPorUsuarioQueryValidator()
        {
            RuleFor(r => r.Login)
                .NotEmpty()
                .WithMessage("Informe o login para obter o total de notificações não lidas");
        }
    }
}
