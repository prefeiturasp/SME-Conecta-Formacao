using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ValidarUsuarioTokenServicoAcessosQuery : IRequest<bool>
    {
        public ValidarUsuarioTokenServicoAcessosQuery(Guid token)
        {
            Token = token;
        }

        public Guid Token { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator : AbstractValidator<ValidarUsuarioTokenServicoAcessosQuery>
    {
        public ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("É necessário informar o token para validar a recuperação de senha");
        }
    }
}
