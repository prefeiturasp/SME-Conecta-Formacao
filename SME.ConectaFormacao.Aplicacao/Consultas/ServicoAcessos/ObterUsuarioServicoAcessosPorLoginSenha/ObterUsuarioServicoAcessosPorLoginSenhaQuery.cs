using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQuery : IRequest<UsuarioAutenticacaoRetornoDTO>
    {
        public ObterUsuarioServicoAcessosPorLoginSenhaQuery(string login, string senha)
        {
            Login = login;
            Senha = senha;
        }

        public string Login { get; }
        public string Senha { get; }
    }

    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryValidator : AbstractValidator<ObterUsuarioServicoAcessosPorLoginSenhaQuery>
    {
        public ObterUsuarioServicoAcessosPorLoginSenhaQueryValidator()
        {
            RuleFor(t => t.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para obter o usuário");

            RuleFor(t => t.Senha)
                .NotEmpty()
                .WithMessage("É necessário informar a senha para obter o usuário");
        }
    }
}
