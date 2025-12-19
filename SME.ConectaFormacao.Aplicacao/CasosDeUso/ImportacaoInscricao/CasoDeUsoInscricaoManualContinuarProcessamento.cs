using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoArquivo.AlterarSituacaoArquivosParaAguardandoProcessamento;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoInscricaoManualContinuarProcessamento(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoInscricaoManualContinuarProcessamento
    {
        public Task<bool> Executar(long arquivoImportacaoId)
        {
            return mediator.Send(new ContinuarProcessamentoDasInscricoesImportadasCommand(arquivoImportacaoId));
        }
    }
}