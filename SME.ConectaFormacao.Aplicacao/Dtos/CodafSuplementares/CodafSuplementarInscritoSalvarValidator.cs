using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    [ExcludeFromCodeCoverage]
    public class CodafSuplementarInscritoSalvarValidator : AbstractValidator<CodafSuplementarInscritoSalvarDto>
    {
        public CodafSuplementarInscritoSalvarValidator()
        {
            RuleFor(c => c.InscricaoId)
                .GreaterThan(0).WithMessage("Inscrito inválido.");
            RuleFor(c => c.PercentualFrequencia)
                .InclusiveBetween(0, 100).When(c => c.PercentualFrequencia.HasValue)
                .WithMessage("Percentual de frequência deve estar entre 0 e 100.");
            RuleFor(c => c.ConceitoFinal)
                .Must(c =>
                {
                    if (string.IsNullOrWhiteSpace(c))
                        return true;
                    var conceito = c!.ToUpper();
                    return conceito == "P" || conceito == "S" || conceito == "NS";
                })
                .WithMessage("Conceito final inválido. Deve ser 'P', 'S' ou 'NS'.");
        }
    }
}

