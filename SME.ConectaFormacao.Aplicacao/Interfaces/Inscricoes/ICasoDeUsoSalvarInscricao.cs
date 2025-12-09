using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoSalvarInscricao
    {
        Task<RetornoDTO> Executar(InscricaoDTO inscricaoDTO);
    }
}
