using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasDashboardQuery : IRequest<IEnumerable<Proposta>>
    {
        public ObterPropostasDashboardQuery(PropostaFiltrosDashboardDTO filtro)
        {
            Filtro = filtro;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; set; }
    }
}