using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCargoFuncao : IRepositorioBaseAuditavel<CargoFuncao>
    {
        Task<IEnumerable<CargoFuncao>> ObterPorTipo(CargoFuncaoTipo? tipo);
    }
}
