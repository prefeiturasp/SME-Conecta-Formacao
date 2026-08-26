using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarEmailEduAoAlterarNomeTipoEmailCommand : IRequest<bool>
    {
        public AlterarEmailEduAoAlterarNomeTipoEmailCommand(string login)
        {
            Login = login;
        }

        public string Login { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarEmailEduAoAlterarNomeCommandValidator : AbstractValidator<AlterarEmailEduAoAlterarNomeTipoEmailCommand>
    {
        public AlterarEmailEduAoAlterarNomeCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().NotEmpty().WithMessage("Informe o login para alterar o e-mail @edu");
        }
    }
}
