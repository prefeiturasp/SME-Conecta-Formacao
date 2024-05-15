using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioPossuiPropostaQueryHandler : IRequestHandler<UsuarioPossuiPropostaQuery, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public UsuarioPossuiPropostaQueryHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public Task<bool> Handle(UsuarioPossuiPropostaQuery request, CancellationToken cancellationToken)
        {
            return _repositorioUsuario.UsuarioPossuiPropostaCadastrada(request.Login);
        }
    }
}
