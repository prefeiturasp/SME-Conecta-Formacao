using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterTipoNotificacao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
