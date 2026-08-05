using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IGerenciadorAnexosCodafCursoNaoHomologadoService
    {
        Task ProcessarAnexosAsync(long codafCursoNaoHomologadoId, IEnumerable<CodafCursoNaoHomologadoAnexo> anexos);
    }
}