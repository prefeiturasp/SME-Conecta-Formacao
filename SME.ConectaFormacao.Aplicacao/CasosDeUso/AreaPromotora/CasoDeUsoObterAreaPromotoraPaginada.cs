using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterAreaPromotoraPaginada
    {
        public CasoDeUsoObterAreaPromotoraPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>> Executar(AreaPromotoraFiltrosDTO filtrosAreaPromotoraDTO)
        {
            return await mediator.Send(new ObterAreasPromotorasPaginadasQuery(filtrosAreaPromotoraDTO, NumeroPagina, NumeroRegistros));
        }
    }
}
