using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQuery : IRequest<long[]>
    {
        public ObterPropostasIdDashboardQuery(PropostaFiltrosDashboardDTO filtro)
        {
            Filtro = filtro;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; set; }
    }
}