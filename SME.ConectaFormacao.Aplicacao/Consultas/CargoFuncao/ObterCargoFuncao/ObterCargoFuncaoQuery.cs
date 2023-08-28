using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoQuery : IRequest<IEnumerable<CargoFuncaoDTO>>
    {
        public ObterCargoFuncaoQuery(CargoFuncaoTipo? tipo)
        {
            Tipo = tipo;
        }

        public CargoFuncaoTipo? Tipo { get; }
    }
}
