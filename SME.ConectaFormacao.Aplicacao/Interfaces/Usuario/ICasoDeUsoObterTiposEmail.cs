using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoObterTiposEmail
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
