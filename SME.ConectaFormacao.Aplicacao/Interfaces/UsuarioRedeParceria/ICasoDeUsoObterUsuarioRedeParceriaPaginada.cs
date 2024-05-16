using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoObterUsuarioRedeParceriaPaginada
    {
        Task<PaginacaoResultadoDTO<UsuarioRedeParceriaPaginadoDTO>> Executar(FiltroUsuarioRedeParceriaDTO filtroUsuarioRedeParceriaDTO);
    }
}
