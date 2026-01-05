using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafInscritoListaPresencaSalvarDto
    {
        public long InscricaoId { get; set; }
        public decimal? PercentualFrequencia { get; set; }
        public string? ConceitoFinal { get; set; }
        public bool? AtividadeObrigatorio { get; set; }
        public bool? Aprovado { get; set; }
    }

    public class CodafInscritoListaPresencaSalvarValidator : AbstractValidator<CodafInscritoListaPresencaSalvarDto>
    {
        public CodafInscritoListaPresencaSalvarValidator()
        {
            RuleFor(c => c.InscricaoId)
                .GreaterThan(0).WithMessage("Inscrição do inscrito inválida.");
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