using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InativarUsuarioCoreSSOServicoAcessosCommand : IRequest<bool>
    {
        public InativarUsuarioCoreSSOServicoAcessosCommand(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class InativarUsuarioCoreSSOServicoAcessosCommandValidator : AbstractValidator<InativarUsuarioCoreSSOServicoAcessosCommand>
    {
        public InativarUsuarioCoreSSOServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para inativar o usuário no coreSSO");
        }
    }
}
