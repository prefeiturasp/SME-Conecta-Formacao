namespace SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces
{
    public interface IServicoAcessos
    {
        Task<AcessosUsuarioAutenticacaoRetorno> Autenticar(string login, string senha);
        Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login);
        Task<bool> UsuarioCadastradoCoreSSO(string login);
        Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha);
        Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId);
        Task<AcessosDadosUsuario> ObterMeusDados(string login);
        Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova);
        Task<bool> AlterarEmail(string login, string email);
        Task<string> SolicitarRecuperacaoSenha(string login);
        Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token);
        Task<string> AlterarSenhaComTokenRecuperacao(Guid token, string novaSenha);
        Task<IEnumerable<AcessosGrupo>> ObterGrupos();
        Task<AcessosGrupo> ObterGrupoPorId(Guid grupoId);
        Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login, Guid perfilUsuarioId);
        Task<AcessosPerfisUsuarioRetorno> RevalidarToken(string token);
    }
}