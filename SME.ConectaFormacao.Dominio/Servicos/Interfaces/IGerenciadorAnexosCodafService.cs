using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IGerenciadorAnexosCodafService
    {
        Task ProcessarAnexosAsync(long codafListaPresencaId, IEnumerable<CodafAnexo> anexos);
    }
}