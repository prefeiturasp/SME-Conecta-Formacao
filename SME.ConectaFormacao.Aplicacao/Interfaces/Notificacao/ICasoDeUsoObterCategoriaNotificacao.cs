using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao
{
    public interface ICasoDeUsoObterCategoriaNotificacao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
