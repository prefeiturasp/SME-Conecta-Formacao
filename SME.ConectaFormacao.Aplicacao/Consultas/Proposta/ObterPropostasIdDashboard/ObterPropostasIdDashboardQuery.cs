using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQuery : IRequest<IEnumerable<Proposta>>
    {
        public ObterPropostasIdDashboardQuery(PropostaFiltrosDashboardDTO filtro, IEnumerable<SituacaoProposta> situacoes, long? areaPromotoraIdUsuarioLogado)
        {
            Filtro = filtro;
            Situacoes = situacoes;
            AreaPromotoraIdUsuarioLogado = areaPromotoraIdUsuarioLogado;
        }

        public PropostaFiltrosDashboardDTO Filtro { get; }
        public IEnumerable<SituacaoProposta> Situacoes { get; }
        public long? AreaPromotoraIdUsuarioLogado { get; }
    }
}