using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterTurmasProposta
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar(long id);
    }
}
