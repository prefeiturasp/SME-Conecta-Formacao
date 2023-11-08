using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioDre : IRepositorioBaseAuditavel<Dre>
    {
        Task<bool> VerificarSeDreExistePorCodigo(string codigoDre);
        Task<Dre> ObterDrePorCodigo(string codigoDre);
    }
}