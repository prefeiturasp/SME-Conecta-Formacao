using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterNotificacao
    {
        Task<NotificacaoDTO> Executar(long id);
    }
}
