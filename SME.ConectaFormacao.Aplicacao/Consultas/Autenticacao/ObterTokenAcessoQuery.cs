using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTokenAcessoQuery : IRequest<UsuarioPerfisRetornoDTO>
    {
        public ObterTokenAcessoQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterTokenAcessoQueryValidator : AbstractValidator<ObterTokenAcessoQuery>
    {
        public ObterTokenAcessoQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É nescessário informar o login para obter o token");
        }
    }
}
