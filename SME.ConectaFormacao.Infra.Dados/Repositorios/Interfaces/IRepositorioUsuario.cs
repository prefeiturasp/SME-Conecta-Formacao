using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioUsuario : IRepositorioBaseAuditavel<Usuario>
    {
        Task<Usuario> ObterPorLogin(string login);
        public Task AtivarCadastroUsuario(long usuarioId);
        Task AtualizarEmailEducacional(long usuarioId, string email);
    }
}
