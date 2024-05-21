using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterSugestaoParecerPareceristas
    {
        Task<IEnumerable<PropostaPareceristaSugestaoDTO>> Executar(long propostaId);
    }
}
