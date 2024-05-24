using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao
{
    public class CasoDeUsoObterNotificacaoPaginada : CasoDeUsoAbstratoPaginado, ICasoDeUsoObterNotificacaoPaginada
    {
        public CasoDeUsoObterNotificacaoPaginada(IMediator mediator, IContextoAplicacao contextoAplicacao) : base(mediator, contextoAplicacao)
        {
        }

        public async Task<PaginacaoResultadoDTO<NotificacaoPaginadoDTO>> Executar(NotificacaoFiltroDTO notificacaoFiltroDTO)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            return await mediator.Send(new ObterNotificacaoPaginadaQuery(
                usuarioLogado.Login, 
                notificacaoFiltroDTO,
                NumeroPagina,
                NumeroRegistros,
                QuantidadeRegistrosIgnorados));
        }
    }
}
