using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterInscricaoFinalizadaPaginada
    {
        Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Executar(InscricaoFinalizadaFiltroDTO inscricaoDTO);
    }
}
