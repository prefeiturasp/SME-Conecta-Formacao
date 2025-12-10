using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuariosRedeParceria.ServicosFakes
{
    public class InativarUsuarioCoreSSOServicoAcessosCommandHandlerFaker : IRequestHandler<InativarUsuarioCoreSSOServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(InativarUsuarioCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
