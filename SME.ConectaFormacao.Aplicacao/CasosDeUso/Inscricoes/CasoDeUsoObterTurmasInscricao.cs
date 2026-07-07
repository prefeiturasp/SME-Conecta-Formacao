using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterTurmasInscricao(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoObterTurmasInscricao
    {
        public async Task<IEnumerable<RetornoListagemDTO>> Executar(long propostaId, string? codigoDre = null, bool comCodaf = false)
        {
            return await mediator.Send(new ObterPropostaTurmasComVagasPorIdQuery(propostaId, codigoDre));
        }
    }
}
