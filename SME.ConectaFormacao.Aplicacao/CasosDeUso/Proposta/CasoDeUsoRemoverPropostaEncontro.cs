using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverPropostaEncontro : CasoDeUsoAbstrato, ICasoDeUsoRemoverPropostaEncontro
    {
        public CasoDeUsoRemoverPropostaEncontro(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new RemoverPropostaEncontroCommand(id));
        }
    }
}
