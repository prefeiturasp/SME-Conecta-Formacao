using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoSalvarPropostaEncontro : CasoDeUsoAbstrato, ICasoDeUsoSalvarPropostaEncontro
    {
        public CasoDeUsoSalvarPropostaEncontro(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(long id, PropostaEncontroDTO propostaEncontroDTO)
        {
            return await mediator.Send(new SalvarPropostaEncontroCommand(id, propostaEncontroDTO));
        }
    }
}
