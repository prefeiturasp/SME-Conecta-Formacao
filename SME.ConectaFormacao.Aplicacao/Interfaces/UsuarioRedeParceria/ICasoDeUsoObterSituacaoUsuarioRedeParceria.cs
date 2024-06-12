using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria
{
    public interface ICasoDeUsoObterSituacaoUsuarioRedeParceria
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
