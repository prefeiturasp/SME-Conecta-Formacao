using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterNotificacaoPaginada
    {
        Task<PaginacaoResultadoDTO<NotificacaoPaginadoDTO>> Executar(NotificacaoFiltroDTO notificacaoFiltroDTO);
    }
}
