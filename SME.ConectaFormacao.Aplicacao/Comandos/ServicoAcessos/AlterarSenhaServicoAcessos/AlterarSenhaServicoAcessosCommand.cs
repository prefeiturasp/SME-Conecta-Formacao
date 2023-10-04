using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSenhaServicoAcessosCommand : IRequest<bool>
    {
        public AlterarSenhaServicoAcessosCommand(string login, string senhaAtual, string novaSenha)
        {
            Login = login;
            SenhaAtual = senhaAtual;
            NovaSenha = novaSenha;
        }

        public string Login { get; }
        public string SenhaAtual { get; }
        public string NovaSenha { get; }
    }

    public class AlterarSenhaServicoAcessosCommandValidator : AbstractValidator<AlterarSenhaServicoAcessosCommand>
    {
        public AlterarSenhaServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário para alterar a senha");

            RuleFor(x => x.SenhaAtual)
                .NotEmpty()
                .WithMessage("É necessário informar a senha atual do usuário para alterar a senha");

            RuleFor(x => x.NovaSenha)
                .NotEmpty()
                .WithMessage("É necessário informar a nova senha do usuário para alterar a senha");
        }
    }
}
