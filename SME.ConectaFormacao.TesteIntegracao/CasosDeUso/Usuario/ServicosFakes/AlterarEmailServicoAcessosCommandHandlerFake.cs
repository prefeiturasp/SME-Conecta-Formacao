using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class AlterarEmailServicoAcessosCommandHandlerFake : IRequestHandler<AlterarEmailServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(AlterarEmailServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
