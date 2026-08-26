using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo
{
    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoImportacaoArquivoCommandValidator : AbstractValidator<AlterarSituacaoImportacaoArquivoCommand>
    {
        public AlterarSituacaoImportacaoArquivoCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador da importação arquivo para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação da importação arquivo para alterar a situação");
        }
    }
}

