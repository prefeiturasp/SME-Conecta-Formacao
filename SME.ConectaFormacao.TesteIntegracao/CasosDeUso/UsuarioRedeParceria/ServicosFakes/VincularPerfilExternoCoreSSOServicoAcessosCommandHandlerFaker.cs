using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes
{
    public class VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker : IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        public Task<bool> Handle(VincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
