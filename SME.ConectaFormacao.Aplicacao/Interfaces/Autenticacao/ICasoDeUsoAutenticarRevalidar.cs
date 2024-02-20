using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao
{
    public interface ICasoDeUsoAutenticarRevalidar
    {
        Task<UsuarioPerfisRetornoDTO> Executar(string token);
    }
}
