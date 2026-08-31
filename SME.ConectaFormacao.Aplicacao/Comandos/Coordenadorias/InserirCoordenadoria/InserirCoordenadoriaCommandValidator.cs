using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria
{

    [ExcludeFromCodeCoverage]
    public class InserirCoordenadoriaCommandValidator : AbstractValidator<InserirCoordenadoriaCommand>
    {
        public InserirCoordenadoriaCommandValidator()
        {
            RuleFor(c => c.Nome).NotEmpty().WithMessage("O nome da coordenadoria é obrigatório.");
            RuleFor(c => c.Sigla).MaximumLength(10).WithMessage("A sigla da coordenadoria deve conter no máximo 10 caracteres.");
        }
    }
}
