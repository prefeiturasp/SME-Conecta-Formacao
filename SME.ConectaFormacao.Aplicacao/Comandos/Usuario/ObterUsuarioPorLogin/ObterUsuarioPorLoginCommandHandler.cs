using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorLoginCommandHandler : IRequestHandler<ObterUsuarioPorLoginCommand, Usuario>
    {
        private readonly IRepositorioUsuario repositorioUsuario;

        public ObterUsuarioPorLoginCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            this.repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorLoginCommand request, CancellationToken cancellationToken)
        {
             return await repositorioUsuario.ObterPorLogin(request.Login);
        }
    }
}
