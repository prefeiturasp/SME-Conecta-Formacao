using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular
{
    public interface ICasoDeUsoObterListaComponentesCurriculares
    {
        Task<IEnumerable<RetornoListagemTodosDTO>> Executar(FiltroListaComponenteCurricularDTO filtroComponenteCurricularDTO);
    }
}
