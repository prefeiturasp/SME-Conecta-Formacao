using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    public class AlterarNomeServicoAcessosCommandHandlerFake : IRequestHandler<AlterarNomeServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(AlterarNomeServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
