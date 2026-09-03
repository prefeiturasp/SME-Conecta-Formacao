using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafCursoNaoHomologadoAnexo : IRepositorioBaseAuditavel<CodafCursoNaoHomologadoAnexo>
    {
        Task<IEnumerable<CodafCursoNaoHomologadoAnexo>> ObterPorCodafCursoNaoHomologadoIdAsync(long codafCursoNaoHomologadoId);
    }
}
