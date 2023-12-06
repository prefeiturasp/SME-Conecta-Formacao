﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ComponenteCurricular
{
    public class CasoDeUsoObterComponentesCurricularesEAnoTurmaEAnoTurmaPorModalidadeAnoLetivoAnoTurmaTurma : CasoDeUsoAbstrato, ICasoDeUsoObterComponentesCurricularesEAnoTurmaPorModalidadeAnoLetivoAnoTurma
    {
        public CasoDeUsoObterComponentesCurricularesEAnoTurmaEAnoTurmaPorModalidadeAnoLetivoAnoTurmaTurma(IMediator mediator) : base(mediator)
        {}

        public async Task<IEnumerable<RetornoListagemTodosDTO>> Executar(ComponenteCurricularEAnoTurmaFiltrosDTO componenteCurricularEAnoTurmaFiltrosDto)
        {
            return await mediator.Send(new ObterComponentesCurricularesEAnoTurmaPorModalidadeAnoIdLetivoAnoTurmaQuery(
                componenteCurricularEAnoTurmaFiltrosDto.Modalidade, 
                componenteCurricularEAnoTurmaFiltrosDto.AnoLetivo, 
                componenteCurricularEAnoTurmaFiltrosDto.AnoId));
        }
    }
}
