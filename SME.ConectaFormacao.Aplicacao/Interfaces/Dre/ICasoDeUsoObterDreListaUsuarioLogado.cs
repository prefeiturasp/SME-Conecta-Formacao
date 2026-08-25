using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Dre
{
    public interface ICasoDeUsoObterDreListaUsuarioLogado
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}
