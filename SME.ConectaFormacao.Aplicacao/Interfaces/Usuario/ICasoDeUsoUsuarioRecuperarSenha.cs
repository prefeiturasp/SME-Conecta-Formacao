using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioRecuperarSenha
    {
        Task<UsuarioPerfisRetornoDTO> Executar(RecuperacaoSenhaDto recuperacaoSenhaDto);
    }
}
