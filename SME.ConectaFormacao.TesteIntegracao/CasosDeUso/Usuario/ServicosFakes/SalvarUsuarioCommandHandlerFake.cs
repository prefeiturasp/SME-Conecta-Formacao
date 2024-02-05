using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao
{
    public class SalvarUsuarioCommandHandlerFake : IRequestHandler<SalvarUsuarioCommand, bool>
    {
        public async Task<bool> Handle(SalvarUsuarioCommand request, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
