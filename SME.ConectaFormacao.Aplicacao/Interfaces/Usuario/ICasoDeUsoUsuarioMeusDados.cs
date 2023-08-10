using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Usuario
{
    public interface ICasoDeUsoUsuarioMeusDados
    {
        Task<DadosUsuarioDTO> Executar(string login);
    }
}
