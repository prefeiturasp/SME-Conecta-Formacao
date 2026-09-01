using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
    public class ObterUsuarioPorLoginQueryValidator : AbstractValidator<ObterUsuarioPorLoginQuery>
    {
        public ObterUsuarioPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para obter o usuário");
        }
    }
}
