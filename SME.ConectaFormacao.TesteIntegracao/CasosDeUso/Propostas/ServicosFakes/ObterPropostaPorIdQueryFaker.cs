using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterPropostaPorIdQueryFaker : IRequestHandler<ObterPropostaPorIdQuery, Dominio.Entidades.Proposta>
    {
        public async Task<Dominio.Entidades.Proposta> Handle(ObterPropostaPorIdQuery request, CancellationToken cancellationToken)
        {
            return new Dominio.Entidades.Proposta();
        }
    }
}