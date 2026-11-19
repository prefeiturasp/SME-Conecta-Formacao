using FluentValidation;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarCadastroDto
    {
        public long CodafId { get; set; }
        public TipoCodafSuplementar Tipo { get; set; } = TipoCodafSuplementar.Homologado;
        public long? CodafCursoNaoHomologadoId { get; set; }
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
            RuleFor(c => c.Tipo).IsInEnum().WithMessage("O tipo do CODAF Suplementar é inválido.");
            RuleFor(c => c.CodafId).GreaterThan(0)
                .When(c => c.Tipo == TipoCodafSuplementar.Homologado)
                .WithMessage("O Id do Codaf é obrigatório.");
            RuleFor(c => c.CodafCursoNaoHomologadoId).NotNull().GreaterThan(0)
                .When(c => c.Tipo == TipoCodafSuplementar.NaoHomologado)
                .WithMessage("O Id do Codaf não homologado é obrigatório.");

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
