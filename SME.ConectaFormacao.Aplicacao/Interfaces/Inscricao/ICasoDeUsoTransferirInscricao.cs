using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoTransferirInscricao
    {
        Task<RetornoInscricaoDTO> Executar(InscricaoTransferenciaDTO inscricaoTransferenciaDTO);
    }
}
