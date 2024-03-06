using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
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
