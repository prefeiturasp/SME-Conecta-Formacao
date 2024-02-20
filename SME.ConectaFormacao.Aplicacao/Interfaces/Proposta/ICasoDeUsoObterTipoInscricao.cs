using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterTipoInscricao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
