using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommand : IRequest<bool>
    {
        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommand(long arquivoImportacaoId)
        {
            ArquivoImportacaoId = arquivoImportacaoId;
        }

        public long ArquivoImportacaoId { get; set; }
    }

    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommandValidator : AbstractValidator<AlterarSituacaoArquivosParaAguardandoProcessamentoCommand>
    {
        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommandValidator()
        {
            RuleFor(x => x.ArquivoImportacaoId).GreaterThan(0).WithMessage("Informe o Id do arquivo de importação para alterar a situação para aguardando processamento");
        }
    }
}
