using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterComunicadoAcaoFormativa
    {
        Task<ComunicadoAcaoFormativaDTO> Executar(long propostaId);
    }
}
