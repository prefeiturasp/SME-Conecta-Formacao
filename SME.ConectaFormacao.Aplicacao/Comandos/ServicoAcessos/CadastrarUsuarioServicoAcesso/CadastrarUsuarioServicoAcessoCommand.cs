using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CadastrarUsuarioServicoAcessoCommand(string login, string nome, string email, string senha) : IRequest<bool>
    {
        public string Login { get; set; } = login;
        public string Nome { get; set; } = nome;
        public string Email { get; set; } = email;
        public string Senha { get; set; } = senha;
        public string? NomeSocial { get; set; }
    }

    [ExcludeFromCodeCoverage]
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
