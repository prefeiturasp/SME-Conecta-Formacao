using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarNomeServicoAcessosCommand : IRequest<bool>
    {
        public AlterarNomeServicoAcessosCommand(string login, string nome)
        {
            Login = login;
            Nome = nome;
        }

        public string Login { get; }
        public string Nome { get; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarNomeServicoAcessosCommandValidator : AbstractValidator<AlterarNomeServicoAcessosCommand>
    {
        public AlterarNomeServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para alterar o email do usuário");

            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o novo nome para ser alterado do usuário");
        }
    }
}

