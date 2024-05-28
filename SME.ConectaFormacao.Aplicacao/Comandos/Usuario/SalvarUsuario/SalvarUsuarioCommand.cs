using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioCommand : IRequest<bool>
    {
        public SalvarUsuarioCommand(Usuario usuario, bool alterouNome = false)
        {
            Usuario = usuario;
            AlterouNome = alterouNome;
        }

        public Usuario Usuario { get; }
        public bool AlterouNome { get; }
    }

    public class SalvarUsuarioCommandValidator : AbstractValidator<SalvarUsuarioCommand>
    {
        public SalvarUsuarioCommandValidator()
        {
            RuleFor(x => x.Usuario)
                .NotNull()
                .WithMessage("É necessário informar o usuário a ser salvo");

            RuleFor(x => x.Usuario.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login do usuário a ser salvo");

            RuleFor(x => x.Usuario.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do usuário a ser salvo");
        }
    }
}
