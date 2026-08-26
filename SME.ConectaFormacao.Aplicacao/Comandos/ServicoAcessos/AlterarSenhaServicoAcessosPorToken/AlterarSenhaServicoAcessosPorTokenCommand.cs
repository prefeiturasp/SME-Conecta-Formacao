using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
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

