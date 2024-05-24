using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoAlterarUsuarioRedeParceria
    {
        Task<RetornoDTO> Executar(long id, UsuarioRedeParceriaDTO usuarioRedeParceriaDTO);
    }
}
