using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterRelatorioPropostaLaudaCompleta : CasoDeUsoAbstrato, ICasoDeUsoObterRelatorioPropostaLaudaCompleta
    {
        public CasoDeUsoObterRelatorioPropostaLaudaCompleta(IMediator mediator) : base(mediator)
        {
        }

        public Task<string> Executar(long propostaId)
        {
            return mediator.Send(new ObterRelatorioProspostaLaudaCompletaQuery(propostaId));
        }
    }
}
