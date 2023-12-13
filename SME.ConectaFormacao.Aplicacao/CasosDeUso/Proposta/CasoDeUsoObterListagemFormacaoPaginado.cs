using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterListagemFormacaoPaginada : CasoDeUsoAbstrato, ICasoDeUsoObterListagemFormacaoPaginada
    {
        public CasoDeUsoObterListagemFormacaoPaginada(IMediator mediator) : base(mediator)
        {}

        public async Task<IEnumerable<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 12;
            
            var propostasIds = await mediator.Send(new ObterPropostasIdsPorFiltroQuery(filtroListagemFormacaoDTO, numeroPagina, numeroRegistros));

            var formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasIds));
            
            return formacoes;
        }
    }
}
