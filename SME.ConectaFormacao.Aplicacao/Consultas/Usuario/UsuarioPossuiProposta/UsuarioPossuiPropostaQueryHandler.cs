using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.UsuarioPossuiProposta
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
