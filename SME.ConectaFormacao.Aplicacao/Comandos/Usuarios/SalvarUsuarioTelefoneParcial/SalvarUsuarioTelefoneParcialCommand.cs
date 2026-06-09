using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioTelefoneParcialCommand : IRequest<bool>
    {
        public SalvarUsuarioTelefoneParcialCommand(string login, string telefone)
        {
            Telefone = telefone;
            Login = login;
        }

        public string Telefone { get; }
        public string Login { get; }
    }

    public class SalvarUsuarioTelefoneParcialCommandValidator : AbstractValidator<SalvarUsuarioTelefoneParcialCommand>
    {
        public SalvarUsuarioTelefoneParcialCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário para alterar usuário");
        }
    }
}
