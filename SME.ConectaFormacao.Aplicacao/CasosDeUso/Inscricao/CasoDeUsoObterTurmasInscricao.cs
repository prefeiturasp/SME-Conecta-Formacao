using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterTurmasInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterTurmasInscricao
    {
        public CasoDeUsoObterTurmasInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar(long propostaId, string? codigoDre = null)
        {
            return await mediator.Send(new ObterPropostaTurmasComVagasPorIdQuery(propostaId, codigoDre));
        }
    }
}
