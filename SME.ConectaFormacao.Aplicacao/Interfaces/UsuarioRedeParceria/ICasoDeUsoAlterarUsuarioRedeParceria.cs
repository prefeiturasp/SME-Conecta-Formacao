using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoAlterarUsuarioRedeParceria
    {
        Task<bool> Executar(long id, UsuarioRedeParceriaDTO usuarioRedeParceriaDTO);
    }
}
