using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ComponenteCurricular
{
    public class CasoDeUsoObterListaComponentesCurriculares : CasoDeUsoAbstrato, ICasoDeUsoObterListaComponentesCurriculares
    {
        public CasoDeUsoObterListaComponentesCurriculares(IMediator mediator) : base(mediator)
        { }

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Executar(FiltroListaComponenteCurricularDTO filtroComponenteCurricularDTO)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorAnoTurmaQuery(filtroComponenteCurricularDTO.AnoTurmaId, filtroComponenteCurricularDTO.ExibirOpcaoTodos));
        }
    }
}
