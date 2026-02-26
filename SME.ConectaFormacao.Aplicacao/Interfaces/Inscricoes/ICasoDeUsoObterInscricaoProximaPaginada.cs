using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterInscricaoProximaPaginada
    {
        Task<PaginacaoResultadoDto<InscricaoPaginadaDTO>> Executar(InscricaoProximaFiltroDTO inscricaoDTO);
    }
}
