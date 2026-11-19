using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IGerenciadorAnexosCodafConferenciaService
    {
        Task ProcessarAnexosAsync(long codafConferenciaId, IEnumerable<CodafConferenciaAnexo> anexos);
    }
}
