using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioTelefoneParcialCommandHandler : IRequestHandler<SalvarUsuarioTelefoneParcialCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public SalvarUsuarioTelefoneParcialCommandHandler(IRepositorioUsuario repositorioUsuario, IMediator mediator)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarUsuarioTelefoneParcialCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login);

            if (usuario == null || usuario.EhNulo())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            usuario.Telefone = request.Telefone.SomenteNumeros();

            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Usuario.Parametros(usuario.Login)), cancellationToken);
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.UsuarioLogado.Parametros(usuario.Login)), cancellationToken);

            return (await _repositorioUsuario.Atualizar(usuario)).Id > 0;
        }
    }
}
