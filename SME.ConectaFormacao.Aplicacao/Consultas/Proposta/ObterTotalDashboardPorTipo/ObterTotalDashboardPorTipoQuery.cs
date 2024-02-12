using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalDashboardPorTipoQuery : IRequest<IEnumerable<QuantidadeTipoDashboardDTO>>
    {
        public ObterTotalDashboardPorTipoQuery(PropostaFiltrosDashboardDTO filtro)
        {
            Filtro = filtro;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; set; }
    }
}