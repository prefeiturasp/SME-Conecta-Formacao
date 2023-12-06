using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular
{
    public interface ICasoDeUsoObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurma
    {
        Task<IEnumerable<RetornoListagemTodosDTO>> Executar(ComponenteCurricularEAnoTurmaFiltrosDTO componenteCurricularEAnoTurmaFiltrosDto);
    }
}
