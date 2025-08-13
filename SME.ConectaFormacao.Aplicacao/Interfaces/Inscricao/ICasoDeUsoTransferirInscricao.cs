using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoTransferirInscricao
    {
        Task<RetornoDTO> Executar(InscricaoTransferenciaDTO inscricaoTransferenciaDTO);
    }
}
