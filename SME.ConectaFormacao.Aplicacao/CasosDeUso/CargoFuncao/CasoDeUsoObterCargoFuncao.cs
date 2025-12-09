using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterCargoFuncao(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoObterCargoFuncao
    {
        public async Task<IEnumerable<CargoFuncaoDto>> Executar(CargoFuncaoTipo? tipo, bool exibirOpcaoOutros)
        {
            return await mediator.Send(new ObterCargoFuncaoPorTipoQuery(tipo, exibirOpcaoOutros));
        }
    }
}
