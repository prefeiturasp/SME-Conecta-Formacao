using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarEmailServicoAcessosCommand : IRequest<bool>
    {
        public AlterarEmailServicoAcessosCommand(string login, string email)
        {
            Login = login;
            Email = email;
        }

        public string Login { get; }
        public string Email { get; }
    }

    public class AlterarEmailServicoAcessosCommandValidator : AbstractValidator<AlterarEmailServicoAcessosCommand>
    {
        public AlterarEmailServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É nescessário informar o login para alterar o email do usuário");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("É nescessário informar o novo email para ser alterado do usuário");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("É nescessário informar o um email válido para ser alterado do usuário");
        }
    }
}
