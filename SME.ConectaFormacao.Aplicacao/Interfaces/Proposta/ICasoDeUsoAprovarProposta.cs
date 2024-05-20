using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoAprovarProposta
    {
        Task<bool> Executar(long propostaId, PropostaJustificativaDTO propostaJustificativaDTO);
    }
}
