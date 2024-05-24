using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoRemoverUsuarioRedeParceria
    {
        Task<RetornoDTO> Executar(long id);
    }
}
