using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterCargoFuncao : CasoDeUsoAbstrato, ICasoDeUsoObterCargoFuncao
    {
        public CasoDeUsoObterCargoFuncao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<CargoFuncaoDTO>> Executar(CargoFuncaoTipo? tipo, bool exibirOpcaoOutros)
        {
            return await mediator.Send(new ObterCargoFuncaoQuery(tipo, exibirOpcaoOutros));
        }
    }
}
