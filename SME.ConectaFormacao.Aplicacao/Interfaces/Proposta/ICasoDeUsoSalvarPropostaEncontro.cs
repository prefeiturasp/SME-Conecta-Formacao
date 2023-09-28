using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoSalvarPropostaEncontro
    {
        Task<long> Executar(long id, PropostaEncontroDTO propostaEncontroDTO);
    }
}
