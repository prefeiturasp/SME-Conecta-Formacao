using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAlterarProposta
    {
        Task<long> Executar(long id, PropostaDTO propostaDTO);
    }
}
