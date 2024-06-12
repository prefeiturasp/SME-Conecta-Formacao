using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    internal class ObterTotalRegistroFilaQueryHandlerFaker : IRequestHandler<ObterTotalRegistroFilaQuery, uint>
    {
        public Task<uint> Handle(ObterTotalRegistroFilaQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult((uint)0);
        }
    }
}
