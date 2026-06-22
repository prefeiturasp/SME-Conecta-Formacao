using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafSuplementarInscricao : IRepositorioBaseAuditavel<CodafSuplementarInscricao>
    {
        Task InserirVariosAsync(IEnumerable<CodafSuplementarInscricao> inscritosSuplementar);
        Task ExcluirPorCodafSuplementarIdAsync(long codafSuplementarId);
    }
}