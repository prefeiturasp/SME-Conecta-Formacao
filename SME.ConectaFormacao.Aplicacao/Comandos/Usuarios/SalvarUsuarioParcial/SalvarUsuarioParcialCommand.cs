using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioParcialCommand(string login, string nome) : IRequest<bool>
    {
        public string Nome { get; } = nome;
        public string Login { get; } = login;
        public string? NomeSocial { get; set; }
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
