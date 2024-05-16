using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoInserirUsuarioRedeParceria
    {
        Task<bool> Executar(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO);
    }
}
