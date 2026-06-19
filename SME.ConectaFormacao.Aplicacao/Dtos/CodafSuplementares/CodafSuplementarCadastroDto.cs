using FluentValidation;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarCadastroDto
    {
        public long CodafId { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataPublicacaoDom { get; set; }
        public short? NumeroComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public IList<CodafSuplementarInscritoSalvarDto>? Inscritos { get; set; }
        public IList<CodafSuplementarRetificacaoSalvarDto>? Retificacoes { get; set; }
        public IList<CodafAnexoSalvarDto>? Anexos { get; set; }
    }

    public class CodafSuplementarCadastroValidator : AbstractValidator<CodafSuplementarCadastroDto>
    {
        public CodafSuplementarCadastroValidator()
        {
            RuleFor(c => c.CodafId).GreaterThan(0).WithMessage("O Id do Codaf é obrigatório.");
            RuleForEach(c => c.Inscritos).SetValidator(new CodafSuplementarInscritoSalvarValidator());
            RuleFor(c => c.Inscritos)
                .Must(inscritos =>
                {
                    if (inscritos is null)
                        return true;
                    var idsInscritos = inscritos.Select(i => i.InscricaoId).ToList();
                    return idsInscritos.Distinct().Count() == idsInscritos.Count;
                })
                .WithMessage("Existem inscritos duplicados no codaf suplementar.");
        }
    }
}
