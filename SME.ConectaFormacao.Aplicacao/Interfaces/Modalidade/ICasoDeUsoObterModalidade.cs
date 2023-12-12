using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade
{
    public interface ICasoDeUsoObterModalidade
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
