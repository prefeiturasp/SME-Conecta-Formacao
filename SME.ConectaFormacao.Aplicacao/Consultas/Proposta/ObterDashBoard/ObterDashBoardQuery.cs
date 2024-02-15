using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDashBoardQuery : IRequest<IEnumerable<PropostaDashboardDTO>>
    {
        public ObterDashBoardQuery(PropostaFiltrosDashboardDTO propostaFiltrosDashboardDTO, long? areaPromotoraIdUsuarioLogado)
        {
            PropostaFiltrosDashboardDTO = propostaFiltrosDashboardDTO;
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
        }

        public PropostaFiltrosDashboardDTO PropostaFiltrosDashboardDTO { get; }

        public long? AreaPromotoraIdUsuarioLogado { get; }
    }
}
