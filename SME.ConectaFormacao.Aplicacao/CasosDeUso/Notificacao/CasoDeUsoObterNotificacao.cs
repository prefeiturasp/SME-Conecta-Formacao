using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao
{
    public class CasoDeUsoObterNotificacao : CasoDeUsoAbstrato, ICasoDeUsoObterNotificacao
    {
        public CasoDeUsoObterNotificacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<NotificacaoDTO> Executar(long id)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            return await mediator.Send(new ObterNotificacaoQuery(id, usuarioLogado.Login));
        }
    }
}
