using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioParametroSistema : IRepositorioBaseAuditavel<ParametroSistema>
    {
        Task<ParametroSistema?> ObterParametroPorTipoEAnoAsync(TipoParametroSistema tipoParametroSistema, int ano);
        Task<ParametroSistema?> ObterParametroPorTipoMaisRecenteAsync(TipoParametroSistema tipoParametroSistema);
        Task<IEnumerable<string>> ObterDominiosPermitidosParaUesParceirasAsync();
    }
}
