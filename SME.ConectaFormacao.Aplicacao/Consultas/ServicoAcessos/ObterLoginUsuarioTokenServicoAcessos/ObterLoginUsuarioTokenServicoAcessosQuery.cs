using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterLoginUsuarioTokenServicoAcessosQuery : IRequest<string>
    {
        public ObterLoginUsuarioTokenServicoAcessosQuery(Guid token, TipoAcao tipoAcao)
        {
            Token = token;
            TipoAcao = tipoAcao;
        }

        public Guid Token { get; }
        public TipoAcao TipoAcao { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterLoginUsuarioTokenServicoAcessosQueryValidator : AbstractValidator<ObterLoginUsuarioTokenServicoAcessosQuery>
    {
        public ObterLoginUsuarioTokenServicoAcessosQueryValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("É necessário informar o token para obter login com base no token");

            RuleFor(x => x.TipoAcao)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo da açaõ para obter login com base no token");
        }
    }
}
