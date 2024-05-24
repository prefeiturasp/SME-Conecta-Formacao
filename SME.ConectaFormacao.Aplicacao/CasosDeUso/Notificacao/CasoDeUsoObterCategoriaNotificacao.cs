using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacao
{
    public class CasoDeUsoObterCategoriaNotificacao : CasoDeUsoAbstrato, ICasoDeUsoObterCategoriaNotificacao
    {
        public CasoDeUsoObterCategoriaNotificacao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterNotificacaoCategoriaQuery.Instancia());
        }
    }
}
