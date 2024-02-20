using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Autenticacao
{
    public interface ICasoDeUsoAutenticarAlterarPerfil
    {
        Task<UsuarioPerfisRetornoDTO> Executar(Guid PerfilUsuarioId);
    }
}
