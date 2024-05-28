using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoEmEsperaInscricoes
    {
        Task<RetornoDTO> Executar(long[] ids);
    }
}
