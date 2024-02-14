using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQuery : IRequest<IEnumerable<Proposta>>
    {
        public ObterPropostasIdDashboardQuery(PropostaFiltrosDashboardDTO filtro,SituacaoProposta situacao)
        {
            Filtro = filtro;
            Situacao = situacao;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; set; }
        public SituacaoProposta Situacao { get; set; }
    }
}