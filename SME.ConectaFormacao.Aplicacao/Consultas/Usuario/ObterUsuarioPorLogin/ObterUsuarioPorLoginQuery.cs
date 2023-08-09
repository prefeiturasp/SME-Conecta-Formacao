using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorLoginQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorLoginQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterUsuarioPorLoginQueryValidator : AbstractValidator<ObterUsuarioPorLoginQuery>
    {
        public ObterUsuarioPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É nescessário informar o login para obter o usuário");
        }
    }
}
