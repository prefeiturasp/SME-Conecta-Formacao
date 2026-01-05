using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoObterDadosPaginadosComFiltros
    {
        Task<PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>> Executar(FiltroListagemInscricaoComTurmaDTO filtro);
    }
}