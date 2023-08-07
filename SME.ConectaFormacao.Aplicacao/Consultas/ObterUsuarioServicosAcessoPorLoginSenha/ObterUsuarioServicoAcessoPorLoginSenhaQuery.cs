using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioServicoAcessoPorLoginSenhaQuery : IRequest<UsuarioAutenticacaoRetornoDTO>
    {
        public ObterUsuarioServicoAcessoPorLoginSenhaQuery(string login, string senha)
        {
            Login = login;
            Senha = senha;
        }

        public string Login { get; }
        public string Senha { get; }
    }

    public class ObterUsuarioServicoAcessoPorLoginSenhaQueryValidator : AbstractValidator<ObterUsuarioServicoAcessoPorLoginSenhaQuery>
    {
        public ObterUsuarioServicoAcessoPorLoginSenhaQueryValidator()
        {
            RuleFor(t => t.Login)
                .Empty()
                .WithMessage("É necessário informar o login para obter o usuário");

            RuleFor(t => t.Senha)
                .Empty()
                .WithMessage("É necessário informar a senha para obter o usuário");
        }
    }
}
