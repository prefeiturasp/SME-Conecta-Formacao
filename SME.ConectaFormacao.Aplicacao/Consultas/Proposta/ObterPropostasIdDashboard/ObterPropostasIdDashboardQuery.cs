using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQuery : IRequest<IEnumerable<long>>
    {
        public ObterPropostasIdDashboardQuery(PropostaFiltrosDashboardDTO filtro, SituacaoProposta situacao, long? areaPromotoraIdUsuarioLogado)
        {
            Filtro = filtro;
            Situacao = situacao;
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; }
        public SituacaoProposta Situacao { get; }
        public long? AreaPromotoraIdUsuarioLogado { get; }
    }
}