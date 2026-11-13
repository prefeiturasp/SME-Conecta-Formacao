using FluentValidation;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafAnexoSalvarDto
    {
        public Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public TipoAnexoCodaf TipoAnexoId { get; set; }
    }

    public class CodafAnexoSalvarValidator : AbstractValidator<CodafAnexoSalvarDto>
    {
        public CodafAnexoSalvarValidator()
        {
            RuleFor(c => c.ArquivoCodigo)
                .NotEmpty()
                .WithMessage("O código do arquivo é obrigatório.");

            RuleFor(c => c.NomeArquivo)
                .NotEmpty()
                .WithMessage("O nome do arquivo é obrigatório.");

            RuleFor(c => c.TipoAnexoId)
                .IsInEnum()
                .WithMessage("Tipo de anexo inválido.");
        }
    }
}
