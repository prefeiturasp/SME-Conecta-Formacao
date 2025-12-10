using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterNotificacaoPaginada
    {
        Task<PaginacaoResultadoDto<NotificacaoPaginadoDTO>> Executar(NotificacaoFiltroDTO notificacaoFiltroDTO);
    }
}
