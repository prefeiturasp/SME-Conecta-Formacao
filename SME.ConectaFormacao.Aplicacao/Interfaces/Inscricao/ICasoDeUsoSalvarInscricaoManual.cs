using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoSalvarInscricaoManual
    {
        Task<RetornoDTO> Executar(InscricaoManualDTO inscricaoManualDTO);
    }
}
