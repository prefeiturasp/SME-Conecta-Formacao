using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPerfisUsuarioServicoAcessosPorLoginQuery : IRequest<UsuarioPerfisRetornoDTO>
    {
        public ObterPerfisUsuarioServicoAcessosPorLoginQuery(string login, Guid? perfilUsuarioId)
        {
            Login = login;
            PerfilUsuarioId = perfilUsuarioId;
        }

        public string Login { get; }
        public Guid? PerfilUsuarioId { get; set; }
    }

    public class ObterPerfisUsuarioServicoAcessosPorLoginQueryValidator : AbstractValidator<ObterPerfisUsuarioServicoAcessosPorLoginQuery>
    {
        public ObterPerfisUsuarioServicoAcessosPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário para obter os perfis");
        }
    }
}
