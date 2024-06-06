using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterSituacaoNotificacao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
