using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtualizarUsuarioServicoAcessoCommand : IRequest<bool>
    {
        public AtualizarUsuarioServicoAcessoCommand(string login, string nome, string email, string senha)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Senha = senha;
        }

        public string Login { get; }
        public string Nome { get; }
        public string Email { get; }
        public string Senha { get; }
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
