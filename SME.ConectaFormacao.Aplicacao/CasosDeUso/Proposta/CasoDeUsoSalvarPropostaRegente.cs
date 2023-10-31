using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaRegente: CasoDeUsoAbstrato,ICasoDeUsoSalvarPropostaRegente
    {
        public CasoDeUsoSalvarPropostaRegente(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(long id, PropostaRegenteDTO propostaRegenteDto)
        {
            return await mediator.Send(new SalvarPropostaRegenteCommand(id, propostaRegenteDto));
        }
    }
}