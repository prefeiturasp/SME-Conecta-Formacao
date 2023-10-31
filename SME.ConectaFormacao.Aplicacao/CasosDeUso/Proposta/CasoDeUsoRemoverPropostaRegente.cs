using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverPropostaRegente: CasoDeUsoAbstrato,ICasoDeUsoRemoverPropostaRegente
    {
        public CasoDeUsoRemoverPropostaRegente(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long regenteId)
        {
            return await mediator.Send(new RemoverPropostaRegenteCommand(regenteId));
        }
    }
}