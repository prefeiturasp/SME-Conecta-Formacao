using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterListagemFormacaoPaginada : CasoDeUsoAbstrato, ICasoDeUsoObterListagemFormacaoPaginada
    {
        public CasoDeUsoObterListagemFormacaoPaginada(IMediator mediator) : base(mediator)
        { }

        public async Task<PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 12;
            var registrosIgnorados = (numeroPagina - 1) * numeroRegistros;

            var propostasIds = await mediator.Send(new ObterPropostasIdsPorFiltroQuery(filtroListagemFormacaoDTO));
            var propostasPaginadas = propostasIds.Take(numeroRegistros).Skip(registrosIgnorados);

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasPaginadas.PossuiElementos())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasPaginadas));

            return new PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>(formacoes, propostasIds.Count(), numeroRegistros);
        }
    }
}
