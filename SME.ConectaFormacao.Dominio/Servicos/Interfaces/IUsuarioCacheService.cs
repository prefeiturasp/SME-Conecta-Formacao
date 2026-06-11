using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IUsuarioCacheService
    {
        Task AtualizarTelefoneEInvalidarCacheAsync(Usuario usuario, string telefone);
    }
}
