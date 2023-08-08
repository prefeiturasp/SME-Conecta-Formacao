using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces
{
    public interface ICasoDeUsoAutenticarUsuario
    {
        Task<UsuarioPerfisRetornoDTO> Executar(AutenticacaoDTO autenticacaoDTO);
    }
}
