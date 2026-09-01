using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class UsuarioPossuiPropostaQuery : IRequest<bool>
    {
        public UsuarioPossuiPropostaQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    [ExcludeFromCodeCoverage]
    public class UsuarioPossuiPropostaQueryValidator : AbstractValidator<UsuarioPossuiPropostaQuery>
    {
        public UsuarioPossuiPropostaQueryValidator()
        {
            RuleFor(f => f.Login)
                .NotEmpty()
                .WithMessage("Informe o login para verificar se possui proposta");
        }
    }
}
