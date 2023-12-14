using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterFormacaoDetalhadaDetalhada : CasoDeUsoAbstrato, ICasoDeUsoObterFormacaoDetalhada
    {
        public CasoDeUsoObterFormacaoDetalhadaDetalhada(IMediator mediator) : base(mediator)
        {}

        public Task<RetornoFormacaoDetalhadaDTO> Executar(long propostaId)
        {
            return mediator.Send(new ObterFormacaoDetalhadaPorIdQuery(propostaId));
        }
    }
}
