using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarNomeServicoAcessosCommandHandler : IRequestHandler<AlterarNomeServicoAcessosCommand, bool>
    {
        private readonly IServicoAcessos _servicoAcessos;

        public AlterarNomeServicoAcessosCommandHandler(IServicoAcessos servicoAcessos)
        {
            _servicoAcessos = servicoAcessos ?? throw new ArgumentNullException(nameof(servicoAcessos));
        }

        public Task<bool> Handle(AlterarNomeServicoAcessosCommand request, CancellationToken cancellationToken)
        {
            return _servicoAcessos.AlterarNome(request.Login, request.Nome);
        }
    }
}
