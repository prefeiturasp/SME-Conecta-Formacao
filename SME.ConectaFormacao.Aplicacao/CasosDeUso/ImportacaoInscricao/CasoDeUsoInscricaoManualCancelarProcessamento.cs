using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo
{
    public class CasoDeUsoInscricaoManualCancelarProcessamento : CasoDeUsoAbstrato, ICasoDeUsoInscricaoManualCancelarProcessamento
    {
        public CasoDeUsoInscricaoManualCancelarProcessamento(IMediator mediator) : base(mediator)
        {
        }

        public Task<bool> Executar(long arquivoImportacaoId)
        {
            return mediator.Send(new AlterarSituacaoArquivosParaCanceladoCommand(arquivoImportacaoId));
        }
    }
}
