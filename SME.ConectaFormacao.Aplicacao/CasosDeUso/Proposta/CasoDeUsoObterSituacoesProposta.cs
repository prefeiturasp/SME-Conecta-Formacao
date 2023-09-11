using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterSituacoesProposta : CasoDeUsoAbstrato, ICasoDeUsoObterSituacoesProposta
    {
        public CasoDeUsoObterSituacoesProposta(IMediator mediator) : base(mediator)
        {
        }

        public Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return mediator.Send(ObterSituacaoPropostaQuery.Instancia);
        }
    }
}
