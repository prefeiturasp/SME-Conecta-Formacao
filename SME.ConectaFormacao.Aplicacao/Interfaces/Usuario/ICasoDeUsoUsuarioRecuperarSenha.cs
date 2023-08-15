using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioRecuperarSenha
    {
        Task<UsuarioPerfisRetornoDTO> Executar(RecuperacaoSenhaDto recuperacaoSenhaDto);
    }
}
