using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterImportacaoArquivoPorIdQuery : IRequest<ImportacaoArquivoDTO>
    {
        public ObterImportacaoArquivoPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
    
    public class ObterImportacaoArquivoPorIdQueryValidator : AbstractValidator<ObterImportacaoArquivoPorIdQuery>
    {
        public ObterImportacaoArquivoPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Informe o Id da importação arquivo");
        }
    }
}