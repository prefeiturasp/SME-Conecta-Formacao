using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public interface ICasoDeUsoObterInscricaoTipo
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}