using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular
{
    public interface ICasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno
    {
        Task<IEnumerable<RetornoListagemTodosDTO>> Executar(ComponenteCurricularFiltrosDTO componenteCurricularFiltrosDto);
    }
}
