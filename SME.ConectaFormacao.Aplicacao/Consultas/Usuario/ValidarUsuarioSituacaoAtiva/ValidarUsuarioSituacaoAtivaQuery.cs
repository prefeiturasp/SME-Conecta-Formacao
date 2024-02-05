using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarUsuarioSituacaoAtivaQuery : IRequest
    {
        public ValidarUsuarioSituacaoAtivaQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

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