using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria
{
    [ExcludeFromCodeCoverage]
    public class AlterarCoordenadoriaCommandValidator : AbstractValidator<AlterarCoordenadoriaCommand>
    {
        public AlterarCoordenadoriaCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id da coordenadoria deve ser maior que zero.");
            RuleFor(c => c.Nome).NotEmpty().WithMessage("Nome da coordenadoria é obrigatório.");
            RuleFor(c => c.Sigla).MaximumLength(10).WithMessage("A sigla da coordenadoria deve conter no máximo 10 caracteres.");
        }
    }
}

