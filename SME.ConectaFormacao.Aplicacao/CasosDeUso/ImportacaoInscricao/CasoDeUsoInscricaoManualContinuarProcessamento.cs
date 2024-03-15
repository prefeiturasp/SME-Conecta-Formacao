using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo
{
    public class CasoDeUsoInscricaoManualContinuarProcessamento : CasoDeUsoAbstrato, ICasoDeUsoInscricaoManualContinuarProcessamento
    {
        public CasoDeUsoInscricaoManualContinuarProcessamento(IMediator mediator) : base(mediator)
        {
        }

        public Task<bool> Executar(long arquivoImportacaoId)
        {
            return mediator.Send(new AlterarSituacaoArquivosParaAguardandoProcessamentoCommand(arquivoImportacaoId));
        }
    }
}
