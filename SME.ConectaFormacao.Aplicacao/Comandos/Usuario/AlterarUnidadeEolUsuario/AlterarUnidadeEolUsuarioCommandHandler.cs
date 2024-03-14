using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarUnidadeEolUsuarioCommandHandler : IRequestHandler<AlterarUnidadeEolUsuarioCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator; 

        public AlterarUnidadeEolUsuarioCommandHandler(IRepositorioUsuario repositorioUsuario,IMediator mediator)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AlterarUnidadeEolUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            usuario.CodigoEolUnidade = request.CodigoEolUnidade;
            await _repositorioUsuario.Atualizar(usuario);

            return true;
        }
        private async Task RemoverCache(string login, string codigoUnidade)
        {
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.UsuarioLogado.Parametros(login)));
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Usuario.Parametros(login)));
        }
    }
}
