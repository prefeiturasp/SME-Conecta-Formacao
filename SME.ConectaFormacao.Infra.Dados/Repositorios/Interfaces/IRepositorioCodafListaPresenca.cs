using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafListaPresenca : IRepositorioBaseAuditavel<CodafListaPresenca>
    {
        Task<bool> TurmaJaTemListaDePresencaAsync(long propostaTurmaId, int listaPresencaId = 0);
    }
}