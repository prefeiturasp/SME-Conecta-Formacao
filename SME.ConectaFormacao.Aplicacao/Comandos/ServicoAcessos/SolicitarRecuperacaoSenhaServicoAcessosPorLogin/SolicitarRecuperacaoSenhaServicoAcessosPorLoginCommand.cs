using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand : IRequest<string>
    {
        public SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand(string login)
        {
            Login = login;
        }

        public string Login { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterTokenRecuperacaoSenhaAcessosPorLoginQueryValidator : AbstractValidator<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand>
    {
        public ObterTokenRecuperacaoSenhaAcessosPorLoginQueryValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para solicitar a recuperação de senha");
        }
    }
}
