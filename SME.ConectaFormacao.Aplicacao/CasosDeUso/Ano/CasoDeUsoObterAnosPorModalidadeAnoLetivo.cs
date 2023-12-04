using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Base;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterAnosPorModalidadeAnoLetivo : CasoDeUsoAbstrato, ICasoDeUsoObterAnosPorModalidadeAnoLetivo
    {
        public CasoDeUsoObterAnosPorModalidadeAnoLetivo(IMediator mediator) : base(mediator)
        {}

        public async Task<IEnumerable<IdNomeOutrosDTO>> Executar(ModalidadeAnoLetivoFiltrosDTO modalidadeAnoLetivoFiltrosDto)
        {
            return await mediator.Send(new ObterAnosPorModalidadeAnoLetivoAnoQuery(
                modalidadeAnoLetivoFiltrosDto.Modalidade, 
                modalidadeAnoLetivoFiltrosDto.AnoLetivo));
        }
    }
}
