using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao
{
    public class CasoDeUsoObterTotalNotificacaoNaoLida : CasoDeUsoAbstrato, ICasoDeUsoObterTotalNotificacaoNaoLida
    {
        public CasoDeUsoObterTotalNotificacaoNaoLida(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar()
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            return await mediator.Send(new ObterTotalNotificacaoNaoLidaPorUsuarioQuery(usuarioLogado.Login));
        }
    }
}
