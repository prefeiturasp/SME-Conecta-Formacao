using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuarioPorCpf
{
    public class ObterUsuarioPorCpfQueryHandler(IRepositorioUsuario repositorioUsuario) : IRequestHandler<ObterUsuarioPorCpfQuery, Dominio.Entidades.Usuario?>
    {
        private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));

        public async Task<Dominio.Entidades.Usuario?> Handle(ObterUsuarioPorCpfQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioUsuario.ObterPorCpf(request.Cpf);
        }
    }
}