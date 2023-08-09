using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPerfisUsuarioServicoAcessosPorLoginQuery : IRequest<UsuarioPerfisRetornoDTO>
    {
        public ObterPerfisUsuarioServicoAcessosPorLoginQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterPerfisUsuarioServicoAcessosPorLoginQueryValidator : AbstractValidator<ObterPerfisUsuarioServicoAcessosPorLoginQuery>
    {
        public ObterPerfisUsuarioServicoAcessosPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .Empty()
                .WithMessage("É necessário informar o login do usuário para obter os perfis");
        }
    }
}
