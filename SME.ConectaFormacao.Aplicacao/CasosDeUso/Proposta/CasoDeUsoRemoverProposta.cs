using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverProposta : CasoDeUsoAbstrato, ICasoDeUsoRemoverProposta
    {
        public CasoDeUsoRemoverProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long id)
        {
            return await mediator.Send(new RemoverPropostaCommand(id));
        }
    }
}
