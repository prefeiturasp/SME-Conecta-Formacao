using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCargoEol
    {
        Task<IEnumerable<CargoEol>> ObterCargosEolPorDreAsync(string codigoDre);
    }
}
