using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioValidacaoEmailToken
    {
        Task<UsuarioPerfisRetornoDTO> Executar(Guid token);
    }
}
