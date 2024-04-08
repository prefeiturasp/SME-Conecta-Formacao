using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorCpfQueryHandler : IRequestHandler<ObterUsuarioPorCpfQuery, Dominio.Entidades.Usuario>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ObterUsuarioPorCpfQueryHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<Dominio.Entidades.Usuario> Handle(ObterUsuarioPorCpfQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioUsuario.ObterPorCpf(request.Cpf);
        }
    }
}