using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterInscricaoPorId
    {
        Task<PaginacaoResultadoDTO<DadosListagemInscricaoDTO>> Executar(long propostaId, FiltroListagemInscricaoDTO filtroListagemInscricaoDTO);
    }
}