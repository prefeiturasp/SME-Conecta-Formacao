using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CargoFuncao
{
    public class CasoDeUsoObterCargoFuncao : CasoDeUsoAbstrato, ICasoDeUsoObterCargoFuncao
    {
        public CasoDeUsoObterCargoFuncao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<CargoFuncaoDTO>> Executar(CargoFuncaoTipo? tipo)
        {
            return await mediator.Send(new ObterCargoFuncaoQuery(tipo));
        }
    }
}
