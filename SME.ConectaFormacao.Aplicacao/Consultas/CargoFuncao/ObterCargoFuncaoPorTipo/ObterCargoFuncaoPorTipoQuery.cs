using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoPorTipoQuery(CargoFuncaoTipo? tipo, bool exibirOutros) : IRequest<IEnumerable<CargoFuncaoDto>>
    {
        public CargoFuncaoTipo? Tipo { get; } = tipo;
        public bool ExibirOutros { get; set; } = exibirOutros;
    }
}
