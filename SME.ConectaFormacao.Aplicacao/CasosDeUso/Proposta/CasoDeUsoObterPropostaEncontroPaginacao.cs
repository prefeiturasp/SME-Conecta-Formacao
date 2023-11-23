using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaEncontroPaginacao : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaEncontroPaginacao
    {
        public CasoDeUsoObterPropostaEncontroPaginacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PaginacaoResultadoDTO<PropostaEncontroDTO>> Executar(long id)
        {
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 10;

            if (id == 0) return new PaginacaoResultadoDTO<PropostaEncontroDTO>(new List<PropostaEncontroDTO>(), 0, 0);

            return await mediator.Send(new ObterEncontrosPaginadoQuery(id, numeroPagina, numeroRegistros));
        }
    }
}
