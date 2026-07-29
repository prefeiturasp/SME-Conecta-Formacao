using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtualizarUsuarioServicoAcessoCommand(string login, string nome, string email, string senha) : IRequest<bool>
    {
        public string Login { get; } = login;
        public string Nome { get; } = nome;
        public string Email { get; } = email;
        public string Senha { get; } = senha;
        public string? NomeSocial { get; set; }
    }

    public class AtualizarUsuarioServicoAcessoCommandValidator : AbstractValidator<AtualizarUsuarioServicoAcessoCommand>
    {
        public AtualizarUsuarioServicoAcessoCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o login para atualizar o usuário no CoreSSO");
            RuleFor(x => x.Nome).NotNull().WithMessage("Informe o nome para atualizar o usuário no CoreSSO");
            RuleFor(x => x.Email).NotNull().WithMessage("Informe o e-mail para atualizar o usuário no CoreSSO");
            RuleFor(x => x.Senha).NotNull().WithMessage("Informe a senha para atualizar o usuário no CoreSSO");
        }
    }
}
