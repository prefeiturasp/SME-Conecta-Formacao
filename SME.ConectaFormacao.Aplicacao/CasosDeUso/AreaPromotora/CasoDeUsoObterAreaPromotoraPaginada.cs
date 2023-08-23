using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraPaginada : CasoDeUsoAbstrato, ICasoDeUsoObterAreaPromotoraPaginada
    {
        private readonly IContextoAplicacao _contextoAplicacao;

        public CasoDeUsoObterAreaPromotoraPaginada(IContextoAplicacao contextoAplicacao, IMediator mediator) : base(mediator)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public async Task<PaginacaoResultadoDTO<AreaPromotoraPaginadaDTO>> Executar(FiltrosAreaPromotoraDTO filtrosAreaPromotoraDTO)
        {
            int numeroPagina = int.TryParse(_contextoAplicacao.ObterVariavel<string>("NumeroPagina"), out numeroPagina) ? numeroPagina : 1;
            int numeroRegistros = int.TryParse(_contextoAplicacao.ObterVariavel<string>("NumeroRegistros"), out numeroRegistros) ? numeroRegistros : 10;

            return await mediator.Send(new ObterAreasPromotorasPaginadasQuery(filtrosAreaPromotoraDTO, numeroPagina, numeroRegistros));
        }
    }
}
