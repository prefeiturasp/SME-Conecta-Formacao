using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao
{
    public class CasoDeUsoObterListagemFormacaoPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterListagemFormacaoPaginada
    {
        public CasoDeUsoObterListagemFormacaoPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            if (NumeroRegistros < 12) NumeroRegistros = 12;

            var propostasIds = await mediator.Send(new ObterPropostasIdsPorFiltroQuery(filtroListagemFormacaoDTO, NumeroPagina, NumeroRegistros));

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasIds.PossuiElementos())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasIds.Distinct()));

            return new PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>(formacoes, propostasIds.Count(), NumeroRegistros);
        }
    }
}
