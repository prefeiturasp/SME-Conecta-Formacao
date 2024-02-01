using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CadastrarUsuarioServicoAcessoCommand : IRequest<bool>
    {
        public CadastrarUsuarioServicoAcessoCommand(string login, string nome, string email, string senha)
        {
            Login = login;
            Nome = nome;
            Email = email;
            Senha = senha;
        }

        public string Login   {get;set;}
        public string Nome {get;set;}
        public string Email {get;set;}
        public string Senha {get;set;}
    }

    public class CadastrarUsuarioServicoAcessoCommandValidator : AbstractValidator<CadastrarUsuarioServicoAcessoCommand>
    {
        public CadastrarUsuarioServicoAcessoCommandValidator()
        {
            RuleFor(x => x.Login).NotNull().WithMessage("Informe o login para Cadastrar o usuário no CoreSSO");
            RuleFor(x => x.Nome).NotNull().WithMessage("Informe o nome para Cadastrar o usuário no CoreSSO");
            RuleFor(x => x.Email).NotNull().WithMessage("Informe o e-mail para Cadastrar o usuário no CoreSSO");
            RuleFor(x => x.Senha).NotNull().WithMessage("Informe a senha para Cadastrar o usuário no CoreSSO");
        }
    }
}
