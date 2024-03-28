using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioUsuario : IRepositorioBaseAuditavel<Usuario>
    {
        Task<Usuario> ObterPorLogin(string login);
        public Task AtivarCadastroUsuario(long usuarioId);
        Task<bool> AtualizarEmailEducacional(string login, string email);
        Task<string?> ObterEmailEducacionalPorLogin(string login);
    }
}
