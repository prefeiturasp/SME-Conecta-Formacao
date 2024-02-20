using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao
{
    public interface ICasoDeUsoAutenticarUsuario
    {
        Task<UsuarioPerfisRetornoDTO> Executar(AutenticacaoDTO autenticacaoDTO);
    }
}
