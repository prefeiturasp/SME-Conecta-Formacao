using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandler : IRequestHandler<DesvincularPerfilExternoCoreSSOServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(DesvincularPerfilExternoCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.DesvincularPerfilExternoCoreSSO(request.Login, request.PerfilId);
        }
    }
}
