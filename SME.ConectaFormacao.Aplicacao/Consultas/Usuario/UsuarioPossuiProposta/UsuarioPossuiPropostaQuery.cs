using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioPossuiPropostaQuery : IRequest<bool>
    {
        public UsuarioPossuiPropostaQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

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
