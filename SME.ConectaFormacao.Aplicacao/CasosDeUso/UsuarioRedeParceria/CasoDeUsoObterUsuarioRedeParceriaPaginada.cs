using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuarioRedeParceria
{
    public class CasoDeUsoObterUsuarioRedeParceriaPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterUsuarioRedeParceriaPaginada
    {
        public CasoDeUsoObterUsuarioRedeParceriaPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>> Executar(FiltroUsuarioRedeParceriaDTO filtroUsuarioRedeParceriaDTO)
        {
            return await mediator.Send(new ObterUsuarioRedeParceiraPaginadaQuery(filtroUsuarioRedeParceriaDTO, NumeroPagina, NumeroRegistros));
        }
    }
}
