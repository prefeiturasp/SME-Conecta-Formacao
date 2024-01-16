using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterDadosPaginadosComFiltros
    {
        Task<PaginacaoResultadoDTO<DadosListagemFormacaoComTurmaDTO>> Executar(FiltroListagemInscricaoComTurmaDTO filtro);
    }
}