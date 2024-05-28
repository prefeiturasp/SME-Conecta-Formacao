using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoCancelarInscricoes
    {
        Task<RetornoDTO> Executar(long[] ids, string motivo);
    }
}
