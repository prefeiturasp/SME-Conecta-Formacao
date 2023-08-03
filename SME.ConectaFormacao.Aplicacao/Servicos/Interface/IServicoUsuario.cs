using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Servicos.Interface
{
    public interface IServicoUsuario : IServicoAplicacao
    {
        Task<long> Inserir(UsuarioDTO usuarioDto);
        Task<IList<UsuarioDTO>> ObterTodos();
        Task<UsuarioDTO> Alterar(UsuarioDTO usuarioDTO);
        Task<UsuarioDTO> ObterPorId(long usuarioId);
        Task<UsuarioAutenticacaoRetornoDTO> Autenticar(string login, string senha);
        Task<UsuarioDTO> ObterPorLogin(string login);
        Task<bool> CadastrarUsuarioExterno(UsuarioExternoDTO usuarioExternoDto);
        Task<DadosUsuarioDTO> ObterMeusDados(string login);
        Task<bool> AlterarSenha(string login, AlterarSenhaUsuarioDTO alterarSenhaUsuarioDto);
        Task<bool> AlterarEmail(string login, string email);
        Task<bool> AlterarEndereco(string login,EnderecoUsuarioExternoDTO enderecoUsuarioExternoDto);
        Task<bool> AlterarTelefone(string login, string telefone);
        Task<string> SolicitarRecuperacaoSenha(string login);
        Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token);
        Task<RetornoPerfilUsuarioDTO?> AlterarSenhaComTokenRecuperacao(RecuperacaoSenhaDto recuperacaoSenhaDto);
    }
}
