using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaParecer : CasoDeUsoAbstrato, ICasoDeUsoSalvarPropostaParecer
    {
        public CasoDeUsoSalvarPropostaParecer(IMediator mediator) : base(mediator)
        {}
        
        public async Task<long> Executar(PropostaParecerDTO propostaParecerDTO)
        {
            return await mediator.Send(new SalvarPropostaParecerCommand(propostaParecerDTO));
        }
    }
}