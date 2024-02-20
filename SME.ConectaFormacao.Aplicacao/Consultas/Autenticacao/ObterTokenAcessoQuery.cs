using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTokenAcessoQuery : IRequest<UsuarioPerfisRetornoDTO>
    {
        public ObterTokenAcessoQuery(string login, Guid? perfilUsuarioId)
        {
            Login = login;
            PerfilUsuarioId = perfilUsuarioId;
        }

        public string Login { get; }
        public Guid? PerfilUsuarioId { get; set; }
    }

    public class ObterTokenAcessoQueryValidator : AbstractValidator<ObterTokenAcessoQuery>
    {
        public ObterTokenAcessoQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para obter o token");
        }
    }
}
