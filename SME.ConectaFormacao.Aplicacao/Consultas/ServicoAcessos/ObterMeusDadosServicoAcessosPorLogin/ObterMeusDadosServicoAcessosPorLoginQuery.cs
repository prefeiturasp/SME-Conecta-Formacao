using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterMeusDadosServicoAcessosPorLoginQuery : IRequest<DadosUsuarioDTO>
    {
        public ObterMeusDadosServicoAcessosPorLoginQuery(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterMeusDadosServicoAcessosPorLoginQueryValidator : AbstractValidator<ObterMeusDadosServicoAcessosPorLoginQuery>
    {
        public ObterMeusDadosServicoAcessosPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para obter o dados do usuário");
        }
    }
}
