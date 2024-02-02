using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtivarUsuarioExternoCommandHandler : IRequestHandler<AtivarUsuarioExternoCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public AtivarUsuarioExternoCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(AtivarUsuarioExternoCommand request, CancellationToken cancellationToken)
        {
            var usuarioExterno = await _repositorioUsuario.ObterPorLogin(request.Login);

            if (usuarioExterno.EhNulo())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            usuarioExterno.Ativar();
            await _repositorioUsuario.Atualizar(usuarioExterno);
            
            return true;
        }
    }
}
