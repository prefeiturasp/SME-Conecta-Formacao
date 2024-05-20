using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoInserirUsuarioRedeParceria
    {
        Task<RetornoDTO> Executar(UsuarioRedeParceriaDTO usuarioRedeParceriaDTO);
    }
}
