using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
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
