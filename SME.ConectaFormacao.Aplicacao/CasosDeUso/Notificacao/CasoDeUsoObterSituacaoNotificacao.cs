using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao
{
    public class CasoDeUsoObterSituacaoNotificacao : CasoDeUsoAbstrato, ICasoDeUsoObterSituacaoNotificacao
    {
        public CasoDeUsoObterSituacaoNotificacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterNotificacaoSituacaoQuery.Instancia());
        }
    }
}
