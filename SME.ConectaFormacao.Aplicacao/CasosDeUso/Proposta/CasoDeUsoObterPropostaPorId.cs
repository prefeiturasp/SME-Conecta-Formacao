using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaPorId : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaPorId
    {
        public CasoDeUsoObterPropostaPorId(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PropostaCompletoDTO> Executar(long id)
        {
            return await mediator.Send(new ObterPropostaPorIdQuery(id));
        }
    }
}
