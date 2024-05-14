using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoInserirUsuarioExterno
    {
        Task<InserirUsuarioRetornoDTO> Executar(UsuarioExternoDTO usuarioExternoDto);
    }
}