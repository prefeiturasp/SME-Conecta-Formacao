using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoEmEsperaInscricoes
    {
        Task<RetornoDTO> Executar(long[] ids);
    }
}
