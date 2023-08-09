using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioCommand : IRequest<bool>
    {
        public SalvarUsuarioCommand(Usuario usuario)
        {
            Usuario = usuario;
        }

        public Usuario Usuario { get; }
    }

    public class SalvarUsuarioCommandValidator : AbstractValidator<SalvarUsuarioCommand>
    {
        public SalvarUsuarioCommandValidator()
        {
            RuleFor(x => x.Usuario)
                .Null()
                .WithMessage("É nescessário informar o usuário a ser salvo");

            RuleFor(x => x.Usuario.Login)
                .Empty()
                .WithMessage("É nescessário informar o login do usuário a ser salvo");

            RuleFor(x => x.Usuario.Nome)
                .Empty()
                .WithMessage("É nescessário informar o nome do usuário a ser salvo");

            RuleFor(x => x.Usuario.Nome)
                .EmailAddress()
                .WithMessage("É nescessário informar um email válido do usuário a ser salvo");
        }
    }
}
