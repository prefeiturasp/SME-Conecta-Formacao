using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IGerenciadorAnexosCodafSuplementarService
    {
        Task ProcessarAnexosAsync(long codafSuplementarId, IEnumerable<CodafSuplementarAnexo> anexos);
    }
}