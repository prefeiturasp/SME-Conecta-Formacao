using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento
{
    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommand : IRequest<bool>
    {
        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommand(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class AlterarSituacaoArquivosParaAguardandoProcessamentoCommandValidator : AbstractValidator<AlterarSituacaoArquivosParaAguardandoProcessamentoCommand>
    {
        public AlterarSituacaoArquivosParaAguardandoProcessamentoCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para alterar a situação dos arquivos para aguardando processamento");
        }
    }
}
