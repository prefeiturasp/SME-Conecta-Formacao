using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoTransferirInscricao
    {
        Task<RetornoInscricaoDTO> Executar(InscricaoTransferenciaDTO inscricaoTransferenciaDTO);
    }
}
