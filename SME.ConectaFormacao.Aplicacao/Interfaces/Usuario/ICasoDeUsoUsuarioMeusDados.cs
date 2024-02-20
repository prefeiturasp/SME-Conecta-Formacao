using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioMeusDados
    {
        Task<DadosUsuarioDTO> Executar(string login);
    }
}
