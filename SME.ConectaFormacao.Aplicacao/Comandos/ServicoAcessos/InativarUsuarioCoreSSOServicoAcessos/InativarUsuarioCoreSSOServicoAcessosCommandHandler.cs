using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InativarUsuarioCoreSSOServicoAcessosCommandHandler : IRequestHandler<InativarUsuarioCoreSSOServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public InativarUsuarioCoreSSOServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public async Task<bool> Handle(InativarUsuarioCoreSSOServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return await _servicoAcessos.InativarUsuario(request.Login);
        }
    }
}
