using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoConfirmarInscricoes
    {
        Task<RetornoDTO> Executar(long[] ids);
    }
}
