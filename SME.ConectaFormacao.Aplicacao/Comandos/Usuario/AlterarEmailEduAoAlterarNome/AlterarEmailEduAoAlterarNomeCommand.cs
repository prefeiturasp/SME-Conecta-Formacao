using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarEmailEduAoAlterarNomeCommand : IRequest<bool>
    {
        public AlterarEmailEduAoAlterarNomeCommand(string login)
        {
            Login = login;
        }

        public string Login { get; set; }
    }

    public class AlterarEmailEduAoAlterarNomeCommandValidator : AbstractValidator<AlterarEmailEduAoAlterarNomeCommand>
    {
        public AlterarEmailEduAoAlterarNomeCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().NotEmpty().WithMessage("Informe o login para alterar o e-mail @edu");
        }
    }
}