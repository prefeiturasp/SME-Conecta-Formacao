using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo
{
    public class CasoDeUsoAlterarSituacaoArquivosParaAguardandoProcessamento : CasoDeUsoAbstrato, ICasoDeUsoAlterarSituacaoArquivosParaAguardandoProcessamento
    {
        public CasoDeUsoAlterarSituacaoArquivosParaAguardandoProcessamento(IMediator mediator) : base(mediator)
        {
        }

        public Task<bool> Executar(long propostaId)
        {
            return mediator.Send(new AlterarSituacaoArquivosParaAguardandoProcessamentoCommand(propostaId));
        }
    }
}
