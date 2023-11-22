using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoParecerProposta
    {
        Task<bool> Executar(long propostaId, ParecerPropostaDTO parecerPropostaDto);
    }
}