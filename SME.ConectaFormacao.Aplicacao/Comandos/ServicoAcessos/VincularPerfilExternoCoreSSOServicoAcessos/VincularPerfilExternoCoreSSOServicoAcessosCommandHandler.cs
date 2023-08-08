using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class VincularPerfilExternoCoreSSOServicoAcessosCommandHandler : IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public VincularPerfilExternoCoreSSOServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(VincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.VincularPerfilExternoCoreSSO(request.Login, request.PerfilId);
        }
    }
}
