using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand : IRequest<bool>
    {
        public EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand(string login)
        {
            Login = login;
        }

        public string Login { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandValidator : AbstractValidator<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>
    {
        public EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o login para poder enviar e-mail de validaçã ao usuário externo recém cadastrado");
        }
    }
}
