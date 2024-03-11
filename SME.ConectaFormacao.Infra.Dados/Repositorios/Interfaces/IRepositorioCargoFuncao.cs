using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCargoFuncao : IRepositorioBaseAuditavel<CargoFuncao>
    {
        Task<bool> ExisteCargoFuncaoOutros(long[] ids);
        Task<CargoFuncao> ObterCargoFuncaoOutros();
        Task<IEnumerable<CargoFuncao>> ObterIgnorandoExcluidosPorTipo(CargoFuncaoTipo? tipo, bool exibirOutros);
        Task<IEnumerable<CargoFuncao>> ObterPorCodigoEol(long[] codigosCargosEol, long[] codigosFuncoesEol);
    }
}
