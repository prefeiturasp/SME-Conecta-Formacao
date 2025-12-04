using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AtivarUsuarioExternoCommandHandler : IRequestHandler<AtivarUsuarioExternoCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly ICacheDistribuido _cacheDistribuido;

        public AtivarUsuarioExternoCommandHandler(IRepositorioUsuario repositorioUsuario, ICacheDistribuido cacheDistribuid)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _cacheDistribuido = cacheDistribuid ?? throw new ArgumentNullException(nameof(cacheDistribuid));
        }

        public async Task<bool> Handle(AtivarUsuarioExternoCommand request, CancellationToken cancellationToken)
        {
            var usuarioExterno = await _repositorioUsuario.ObterPorLogin(request.Login);

            if (usuarioExterno.EhNulo())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            await _repositorioUsuario.AtivarCadastroUsuario(usuarioExterno.Id);

            await _cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.Usuario.Parametros(request.Login));

            return true;
        }
    }
}