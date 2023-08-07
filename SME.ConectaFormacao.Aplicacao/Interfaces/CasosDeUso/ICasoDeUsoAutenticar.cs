using SME.ConectaFormacao.Aplicacao.DTOS;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CasosDeUso
{
    public interface ICasoDeUsoAutenticar
    {
        Task<UsuarioAutenticacaoRetornoDTO> Executar(string login, string senha);
    }
}
