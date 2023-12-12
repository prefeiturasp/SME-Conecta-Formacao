using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Ano;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Ano
{
    public interface ICasoDeUsoObterListaAnoTurma
    {
        Task<IEnumerable<RetornoListagemTodosDTO>> Executar(FiltroAnoTurmaDTO filtroAnoTurmaDTO);
    }
}
