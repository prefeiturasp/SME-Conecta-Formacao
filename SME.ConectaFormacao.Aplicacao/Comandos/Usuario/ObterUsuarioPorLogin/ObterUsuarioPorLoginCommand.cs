using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorLoginCommand : IRequest<Usuario>
    {
        public string Login { get; set; }

        public ObterUsuarioPorLoginCommand(string login)
        {
            Login = login;
        }
    }
    public class ObterUsuarioPorLoginCommandValidator : AbstractValidator<ObterUsuarioPorLoginCommand>
    {
        public ObterUsuarioPorLoginCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o Login para Obter o usuário");
        }
    }
}
