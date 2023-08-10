using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao
{
    public interface ICasoDeUsoAutenticarUsuario
    {
        Task<UsuarioPerfisRetornoDTO> Executar(AutenticacaoDTO autenticacaoDTO);
    }
}
