using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoObterUsuarioRedeParceriaPorId
    {
        Task<UsuarioRedeParceriaDTO> Executar(long id);
    }
}
