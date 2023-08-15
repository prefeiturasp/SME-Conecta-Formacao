using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand : IRequest<string>
    {
        public SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    public class ObterTokenRecuperacaoSenhaAcessosPorLoginQueryValidator : AbstractValidator<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>
    {
        public ObterTokenRecuperacaoSenhaAcessosPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É nescessário informar o login para solicitar a recuperação de senha");
        }
    }
}
