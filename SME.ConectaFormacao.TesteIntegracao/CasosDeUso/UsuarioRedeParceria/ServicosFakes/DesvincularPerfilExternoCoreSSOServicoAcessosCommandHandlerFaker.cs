using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes
{
    internal class DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker : IRequestHandler<DesvincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(DesvincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
