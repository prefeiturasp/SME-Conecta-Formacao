using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Formacao
{
    public interface ICasoDeUsoObterListagemFormacaoPaginada
    {
        Task<PaginacaoResultadoDTO<RetornoListagemFormacaoDTO>> Executar(FiltroListagemFormacaoDTO filtroListagemFormacaoDTO);
    }
}
