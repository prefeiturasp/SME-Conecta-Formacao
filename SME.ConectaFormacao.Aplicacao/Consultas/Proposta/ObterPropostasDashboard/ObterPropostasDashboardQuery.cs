using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasDashboardQuery : IRequest<IEnumerable<Proposta>>
    {
        public ObterPropostasDashboardQuery(long[] propostasIds)
        {
            PropostasIds = propostasIds;
        }

        public long[] PropostasIds { get; set; }
    }
}