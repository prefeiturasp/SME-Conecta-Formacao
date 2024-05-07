using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterRelatorioPropostaLaudaPublicacao : CasoDeUsoAbstrato, ICasoDeUsoObterRelatorioPropostaLaudaPublicacao
    {
        public CasoDeUsoObterRelatorioPropostaLaudaPublicacao(IMediator mediator) : base(mediator)
        {
        }

        public Task<string> Executar(long propostaId)
        {
            return mediator.Send(new ObterRelatorioProspostaLaudaPublicacaoQuery(propostaId));
        }
    }
}
