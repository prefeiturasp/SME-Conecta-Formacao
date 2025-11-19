using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarEmailEducacional
{
    public class AlterarEmailEducacionalCommand : IRequest<bool>
    {
        public AlterarEmailEducacionalCommand(string email, string login)
        {
            Email = email;
            Login = login;
        }

        public string Email { get; set; }
        public string Login { get; set; }
    }

    public class AlterarEmailEducacionalCommandValidator : AbstractValidator<AlterarEmailEducacionalCommand>
    {
        public AlterarEmailEducacionalCommandValidator()
        {
            RuleFor(x => x.Email).NotNull().WithMessage("Informe o E-mail Educacional");
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o Login para alterar o E-mail Educacional");
        }
    }
}