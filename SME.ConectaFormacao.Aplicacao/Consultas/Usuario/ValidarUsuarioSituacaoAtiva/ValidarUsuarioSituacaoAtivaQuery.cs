using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ValidarUsuarioSituacaoAtivaQuery : IRequest
    {
        public ValidarUsuarioSituacaoAtivaQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ValidarUsuarioSituacaoAtivaQueryValidator : AbstractValidator<ValidarUsuarioSituacaoAtivaQuery>
    {
        public ValidarUsuarioSituacaoAtivaQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para validar se o usuário está com situação ativa");
        }
    }
}