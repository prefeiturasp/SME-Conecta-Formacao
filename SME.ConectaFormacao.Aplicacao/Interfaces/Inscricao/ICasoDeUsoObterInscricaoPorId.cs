using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterInscricaoPorId
    {
        Task<IEnumerable<DadosListagemInscricaoDTO>> Executar(long inscricaoId,FiltroListagemInscricaoDTO filtroListagemInscricaoDTO);
    }
}