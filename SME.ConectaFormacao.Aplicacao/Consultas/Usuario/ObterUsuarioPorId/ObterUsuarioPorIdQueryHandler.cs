using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorIdQueryHandler : IRequestHandler<ObterUsuarioPorIdQuery, Usuario>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterUsuarioPorIdQueryHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioUsuario.ObterPorId(request.UsuarioId);
        }
    }
}