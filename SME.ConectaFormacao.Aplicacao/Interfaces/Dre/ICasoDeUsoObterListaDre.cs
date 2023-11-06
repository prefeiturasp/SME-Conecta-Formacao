using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Dre
{
    public interface ICasoDeUsoObterListaDre
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}