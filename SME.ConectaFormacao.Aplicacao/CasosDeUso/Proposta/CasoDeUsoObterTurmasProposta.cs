using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTurmasProposta : CasoDeUsoAbstrato, ICasoDeUsoObterTurmasProposta
    {
        public CasoDeUsoObterTurmasProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar(long id)
        {
            return await mediator.Send(new ObterPropostaTurmasPorIdQuery(id));
        }
    }
}
