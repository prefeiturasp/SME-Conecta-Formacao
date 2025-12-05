using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverUsuarioCommandHandler : IRequestHandler<RemoverUsuarioCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public RemoverUsuarioCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(RemoverUsuarioCommand request, CancellationToken cancellationToken)
        {
            await _repositorioUsuario.Remover(request.Id);
            return true;
        }
    }
}
