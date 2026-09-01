using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RevalidarTokenServicoAcessosQuery : IRequest<UsuarioPerfisRetornoDTO>
    {
        public RevalidarTokenServicoAcessosQuery(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }

    [ExcludeFromCodeCoverage]
    public class RevalidarTokenServicoAcessosQueryValidator : AbstractValidator<RevalidarTokenServicoAcessosQuery>
    {
        public RevalidarTokenServicoAcessosQueryValidator()
        {
            RuleFor(t => t.Token)
                .NotEmpty()
                .WithMessage("Informe o token para ser revalidado");
        }
    }
}
