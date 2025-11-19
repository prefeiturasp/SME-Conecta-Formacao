using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarTipoEmail
{
    public class AlterarTipoEmailCommand : IRequest<bool>
    {
        public AlterarTipoEmailCommand(int tipo, string login)
        {
            Tipo = tipo;
            Login = login;
        }

        public int Tipo { get; set; }
        public string Login { get; set; }
    }

    public class AlterarTipoEmailCommandValidator : AbstractValidator<AlterarTipoEmailCommand>
    {
        public AlterarTipoEmailCommandValidator()
        {
            RuleFor(x => x.Tipo).NotNull().WithMessage("Informe o Tipo E-mail");
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o Login para alterar o E-mail Educacional");
        }
    }
}