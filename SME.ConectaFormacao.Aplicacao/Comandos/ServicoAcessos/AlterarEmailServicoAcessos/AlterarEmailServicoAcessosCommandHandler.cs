using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarEmailServicoAcessosCommandHandler : IRequestHandler<AlterarEmailServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public AlterarEmailServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(AlterarEmailServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.AlterarEmail(request.Login, request.Email);
        }
    }
}
