using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
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
