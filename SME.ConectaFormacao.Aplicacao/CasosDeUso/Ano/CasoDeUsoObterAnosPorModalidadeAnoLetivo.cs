using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ComponenteCurricular
{
    public class CasoDeUsoObterAnosPorModalidadeAnoLetivo : CasoDeUsoAbstrato, ICasoDeUsoObterAnosPorModalidadeAnoLetivo
    {
        public CasoDeUsoObterAnosPorModalidadeAnoLetivo(IMediator mediator) : base(mediator)
        {}

        public async Task<IEnumerable<IdNomeTodosDTO>> Executar(ModalidadeAnoLetivoFiltrosDTO modalidadeAnoLetivoFiltrosDto)
        {
            return await mediator.Send(new ObterAnosPorModalidadeAnoLetivoQuery(
                modalidadeAnoLetivoFiltrosDto.Modalidade, 
                modalidadeAnoLetivoFiltrosDto.AnoLetivo));
        }
    }
}
