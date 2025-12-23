using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafListaPresencaEdicaoDto
    {
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public DateOnly? DataPublicacao { get; set; }
        public DateOnly? DataPublicacaoDom { get; set; }
        public short? NumeroComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public IList<CodafInscritoListaPresencaSalvarDto>? Inscritos { get; set; }
    }

    public class CodafListaPresencaEdicaoValidator : AbstractValidator<CodafListaPresencaEdicaoDto>
    {
        public CodafListaPresencaEdicaoValidator()
        {
            RuleForEach(c => c.Inscritos).SetValidator(new CodafInscritoListaPresencaSalvarValidator());
            RuleFor(c => c.Inscritos)
                .Must(inscritos =>
                {
                    if (inscritos is null)
                        return true;
                    var idsInscritos = inscritos.Select(i => i.InscricaoId).ToList();
                    return idsInscritos.Distinct().Count() == idsInscritos.Count;
                })
                .WithMessage("Existem inscritos duplicados na lista de presença.");
        }
    }
}