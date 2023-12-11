using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CriterioCertificacao
{
    public interface ICasoDeUsoCriterioCertificacao
    {
        Task<IEnumerable<RetornoListagemDTO>> Executar();
    }
}