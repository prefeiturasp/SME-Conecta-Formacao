using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioParcialCommandHandler(IRepositorioUsuario repositorioUsuario, IMediator mediator) : IRequestHandler<SalvarUsuarioParcialCommand, bool>
    {
        public async Task<bool> Handle(SalvarUsuarioParcialCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new NegocioException(MensagemNegocio.NOME_USUARIO_NAO_PREENCHIDO);

            var usuario = await repositorioUsuario.ObterPorLogin(request.Login) ?? 
                          throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            
            usuario.Nome = request.Nome.Trim();
            usuario.NomeSocial = request.NomeSocial?.Trim();

            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Usuario.Parametros(usuario.Login)), cancellationToken);
            await mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.UsuarioLogado.Parametros(usuario.Login)), cancellationToken);

            return (await repositorioUsuario.Atualizar(usuario)).Id > 0;
        }
    }
}
