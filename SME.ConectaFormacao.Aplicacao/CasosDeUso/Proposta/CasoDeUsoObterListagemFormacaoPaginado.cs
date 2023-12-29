using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterListagemFormacaoPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterListagemFormacaoPaginada
    {
        public CasoDeUsoObterListagemFormacaoPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO)
        {
            if (NumeroRegistros < 12) NumeroRegistros = 12;

            var registrosIgnorados = (NumeroPagina - 1) * NumeroRegistros;

            var propostasIds = await mediator.Send(new ObterPropostasIdsPorFiltroQuery(filtroListagemFormacaoDTO));
            var propostasPaginadas = propostasIds.Take(NumeroRegistros).Skip(registrosIgnorados);

            var formacoes = Enumerable.Empty<RetornoListagemFormacaoDTO>();
            if (propostasPaginadas.PossuiElementos())
                formacoes = await mediator.Send(new ObterPropostasPorIdsQuery(propostasPaginadas));

            return new PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>(formacoes, propostasIds.Count(), NumeroRegistros);
        }
    }
}
