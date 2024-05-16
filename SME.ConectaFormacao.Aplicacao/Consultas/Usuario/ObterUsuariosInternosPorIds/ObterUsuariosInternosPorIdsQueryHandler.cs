using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosInternosPorIdsQueryHandler : IRequestHandler<ObterUsuariosInternosPorIdsQuery, IEnumerable<Usuario>>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterUsuariosInternosPorIdsQueryHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<IEnumerable<Usuario>> Handle(ObterUsuariosInternosPorIdsQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioUsuario.ObterUsuarioInternoPorId(request.UsuariosId);
        }
    }
}