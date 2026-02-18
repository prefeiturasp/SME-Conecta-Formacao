using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioUsuarioAcessibilidade : IRepositorioBaseAuditavel<UsuarioAcessibilidade>
    {
        Task<UsuarioAcessibilidade?> ObterPorUsuarioIdAsync(long usuarioId);
    }
}