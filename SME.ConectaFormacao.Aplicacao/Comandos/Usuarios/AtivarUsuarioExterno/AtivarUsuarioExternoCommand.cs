using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtivarUsuarioExternoCommand : IRequest<bool>
    {
        public AtivarUsuarioExternoCommand(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class AtivarUsuarioExternoCommandValidator : AbstractValidator<AtivarUsuarioExternoCommand>
    {
        public AtivarUsuarioExternoCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotNull()
                .WithMessage("É necessário informar o login do usuário externo para ativá-lo");
        }
    }
}
