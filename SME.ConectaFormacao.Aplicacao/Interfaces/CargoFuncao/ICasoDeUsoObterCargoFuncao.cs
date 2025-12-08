using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao
{
    public interface ICasoDeUsoObterCargoFuncao
    {
        Task<IEnumerable<CargoFuncaoDto>> Executar(CargoFuncaoTipo? tipo, bool exibirOpcaoOutros);
    }
}
