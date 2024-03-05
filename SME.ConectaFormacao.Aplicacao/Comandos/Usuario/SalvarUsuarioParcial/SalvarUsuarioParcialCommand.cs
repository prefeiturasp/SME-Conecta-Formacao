using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioParcialCommand : IRequest<bool>
    {
        public SalvarUsuarioParcialCommand(string login, string nome)
        {
            Nome = nome;
            Login = login;
        }

        public string Nome { get; }
        public string Login { get; }
    }

    public class SalvarUsuarioParcialCommandValidator : AbstractValidator<SalvarUsuarioParcialCommand>
    {
        public SalvarUsuarioParcialCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário para alterar usuário");
            
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do usuário para alterar usuário");
        }
    }
}
