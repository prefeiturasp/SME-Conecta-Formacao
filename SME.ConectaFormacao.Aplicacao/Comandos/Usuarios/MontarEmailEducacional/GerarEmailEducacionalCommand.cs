using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarEmailEducacionalCommand : IRequest<string>
    {
        public GerarEmailEducacionalCommand(Usuario usuario)
        {
            Usuario = usuario;
        }

        public Usuario Usuario { get; set; }
    }

    public class MontarEmailEducacionalCommandValidator : AbstractValidator<GerarEmailEducacionalCommand>
    {
        public MontarEmailEducacionalCommandValidator()
        {
            RuleFor(x => x.Usuario)
                .NotNull()
                .WithMessage("Informe o Usuário para criar o e-mail educacional");
        }
    }
}