using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterPropostasDashboard
    {
        Task<IEnumerable<PropostaDashboardDTO>> Executar(PropostaFiltrosDashboardDTO filtro);
    }
}