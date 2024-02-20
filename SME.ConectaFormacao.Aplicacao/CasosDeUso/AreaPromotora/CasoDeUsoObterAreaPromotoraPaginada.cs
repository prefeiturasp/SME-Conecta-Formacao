using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraPaginada : CasoDeUsoAbstrato, ICasoDeUsoObterAreaPromotoraPaginada
    {
        public CasoDeUsoObterAreaPromotoraPaginada(IMediator mediator) : base(mediator)
        {
        }

        public async Task<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>> Executar(AreaPromotoraFiltrosDTO filtrosAreaPromotoraDTO)
        {
            int numeroPagina = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroPagina")), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(await mediator.Send(new ObterVariavelContextoAplicacaoQuery("NumeroRegistros")), out numeroRegistros) ? numeroRegistros : 10;

            return await mediator.Send(new ObterAreasPromotorasPaginadasQuery(filtrosAreaPromotoraDTO, numeroPagina, numeroRegistros));
        }
    }
}
