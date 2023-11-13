using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoObterListaDre
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}