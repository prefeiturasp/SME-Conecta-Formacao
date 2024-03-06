using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAlterarProposta
    {
        Task<RetornoDTO> Executar(long id, PropostaDTO propostaDTO);
    }
}
