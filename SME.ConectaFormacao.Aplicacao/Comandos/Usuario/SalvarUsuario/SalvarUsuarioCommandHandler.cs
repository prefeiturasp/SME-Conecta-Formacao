using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioCommandHandler : IRequestHandler<SalvarUsuarioCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public SalvarUsuarioCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(SalvarUsuarioCommand request, CancellationToken cancellationToken)
        {
            if (request.Usuario.Id > 0)
                return await _repositorioUsuario.Atualizar(request.Usuario) != null;
            else
                return await _repositorioUsuario.Inserir(request.Usuario) > 0;
        }
    }
}
