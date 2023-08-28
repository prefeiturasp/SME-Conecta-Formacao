using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterCargoFuncao
    {
        Task<IEnumerable<CargoFuncaoDTO>> Executar(CargoFuncaoTipo? tipo);
    }
}
