using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSenhaServicoAcessosPorTokenCommand : IRequest<string>
    {
        public AlterarSenhaServicoAcessosPorTokenCommand(Guid token, string novaSenha)
        {
            Token = token;
            NovaSenha = novaSenha;
        }

        public Guid Token { get; }
        public string NovaSenha { get; }
    }

    public class AlterarSenhaServicoAcessosPorTokenCommandValidator : AbstractValidator<AlterarSenhaServicoAcessosPorTokenCommand>
    {
        public AlterarSenhaServicoAcessosPorTokenCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("É necessário informar o token de recuperação de senha para alterar a senha");

            RuleFor(x => x.NovaSenha)
                .NotEmpty()
                .WithMessage("É necessário informar a nova senha de recuperação de senha para alterar");
        }
    }
}
