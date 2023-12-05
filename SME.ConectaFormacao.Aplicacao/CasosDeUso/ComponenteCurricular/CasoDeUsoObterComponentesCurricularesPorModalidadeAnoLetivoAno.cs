﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno : CasoDeUsoAbstrato, ICasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno
    {
        public CasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno(IMediator mediator) : base(mediator)
        {}

        public async Task<IEnumerable<IdNomeTodosDTO>> Executar(ComponenteCurricularFiltrosDto componenteCurricularFiltrosDto)
        {
            return await mediator.Send(new ObterComponentesCurricularesPorModalidadeAnoIdLetivoAnoQuery(
                componenteCurricularFiltrosDto.Modalidade, 
                componenteCurricularFiltrosDto.AnoLetivo, 
                componenteCurricularFiltrosDto.AnoId));
        }
    }
}
