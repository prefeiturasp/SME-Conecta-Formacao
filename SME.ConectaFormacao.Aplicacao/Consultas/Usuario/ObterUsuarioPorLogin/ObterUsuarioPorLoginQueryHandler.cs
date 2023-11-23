using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorLoginQueryHandler : IRequestHandler<ObterUsuarioPorLoginQuery, Usuario>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterUsuarioPorLoginQueryHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public Task<Usuario> Handle(ObterUsuarioPorLoginQuery request, CancellationToken cancellationToken)
        {
            return _repositorioUsuario.ObterPorLogin(request.Login);
        }
    }
}
