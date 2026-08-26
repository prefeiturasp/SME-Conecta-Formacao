using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoArquivosParaCanceladoCommand : IRequest<bool>
    {
        public AlterarSituacaoArquivosParaCanceladoCommand(long arquivoImportacaoId)
        {
            ArquivoImportacaoId = arquivoImportacaoId;
        }

        public long ArquivoImportacaoId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoArquivosParaCanceladoCommandValidator : AbstractValidator<AlterarSituacaoArquivosParaCanceladoCommand>
    {
        public AlterarSituacaoArquivosParaCanceladoCommandValidator()
        {
            RuleFor(x => x.ArquivoImportacaoId).GreaterThan(0).WithMessage("Informe o Id do arquivo de importação para alterar a situação para cancelado");
        }
    }
}

