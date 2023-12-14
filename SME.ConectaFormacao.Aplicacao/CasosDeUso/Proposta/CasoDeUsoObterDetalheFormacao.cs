using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterDetalheFormacao : CasoDeUsoAbstrato, ICasoDeUsoObterDetalheFormacao
    {
        public CasoDeUsoObterDetalheFormacao(IMediator mediator) : base(mediator)
        {}

        public Task<RetornoDetalheFormacaoDTO> Executar(long propostaId)
        {
            return mediator.Send(new ObterPropostaDetalhePorIdQuery(propostaId));
        }
    }
}
