using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterParecerProposta : CasoDeUsoAbstrato, ICasoDeUsoObterParecerProposta
    {
        public CasoDeUsoObterParecerProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PropostaMovimentacaoDTO> Executar(long propostaId)
        {
            return await mediator.Send(new ObterUltimoParecerPropostaQuery(propostaId));
        }
    }
}
