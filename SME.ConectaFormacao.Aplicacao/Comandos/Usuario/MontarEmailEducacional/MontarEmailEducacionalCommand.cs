using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class MontarEmailEducacionalCommand : IRequest<string>
    {
        public MontarEmailEducacionalCommand(string nomeCompleto)
        {
            NomeCompleto = nomeCompleto ?? throw new ArgumentNullException(nameof(nomeCompleto));
        }

        public string NomeCompleto { get; set; }    
    }

    public class MontarEmailEducacionalCommandValidator : AbstractValidator<MontarEmailEducacionalCommand>
    {
        public MontarEmailEducacionalCommandValidator()
        {
            RuleFor(x => x.NomeCompleto)
                .NotNull()
                .NotEmpty()
                .WithMessage("Informe o nome completo do Usuario para montar o e-mail educacional");
        }
    }
}