using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTotalRegistroFilaQueryHandler : IRequestHandler<ObterTotalRegistroFilaQuery, uint>
    {
        private readonly IConexoesRabbit _conexoesRabbit;

        public ObterTotalRegistroFilaQueryHandler(IConexoesRabbit conexoesRabbit)
        {
            _conexoesRabbit = conexoesRabbit ?? throw new ArgumentNullException(nameof(conexoesRabbit));
        }

        public Task<uint> Handle(ObterTotalRegistroFilaQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_conexoesRabbit.Get().MessageCount(request.Fila));
        }
    }
}
