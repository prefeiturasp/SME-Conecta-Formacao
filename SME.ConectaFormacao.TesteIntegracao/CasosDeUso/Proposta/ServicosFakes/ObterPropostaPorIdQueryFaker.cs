using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterPropostaPorIdQueryFaker : IRequestHandler<ObterPropostaPorIdQuery, Dominio.Entidades.Proposta>
    {
        public async Task<Dominio.Entidades.Proposta> Handle(ObterPropostaPorIdQuery request, CancellationToken cancellationToken)
        {
            return new Dominio.Entidades.Proposta();
        }
    }
}