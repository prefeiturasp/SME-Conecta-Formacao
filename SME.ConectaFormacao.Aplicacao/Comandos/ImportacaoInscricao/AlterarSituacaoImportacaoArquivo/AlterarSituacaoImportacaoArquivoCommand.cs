using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoImportacaoArquivoCommand : IRequest<bool>
    {
        public AlterarSituacaoImportacaoArquivoCommand(long importacaoArquivoId, SituacaoImportacaoArquivo situacao)
        {
            ImportacaoArquivoId = importacaoArquivoId;
            Situacao = situacao;
        }

        public long ImportacaoArquivoId { get; }
        public SituacaoImportacaoArquivo Situacao { get; }
    }

    public class AlterarSituacaoImportacaoArquivoCommandValidator : AbstractValidator<AlterarSituacaoImportacaoArquivoCommand>
    {
        public AlterarSituacaoImportacaoArquivoCommandValidator()
        {
            RuleFor(x => x.ImportacaoArquivoId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador da importação arquivo para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação da importação arquivo para alterar a situação");
        }
    }
}
