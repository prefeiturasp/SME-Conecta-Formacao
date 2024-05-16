using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioCommandHandler : IRequestHandler<SalvarUsuarioCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public SalvarUsuarioCommandHandler(IRepositorioUsuario repositorioUsuario, IMediator mediator)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(SalvarUsuarioCommand request, CancellationToken cancellationToken)
        {
            await AtualizarEmail(request.Usuario);
            if (request.Usuario.Id > 0)
            {
                await RemoverCache(request.Usuario.Login);
                return await _repositorioUsuario.Atualizar(request.Usuario) != null;
            }
            else
                return await _repositorioUsuario.Inserir(request.Usuario) > 0;
        }
        private async Task AtualizarEmail(Usuario usuario)
        {
           if(usuario.EmailEducacional.NaoEstaPreenchido())
               usuario.EmailEducacional = await _mediator.Send(new GerarEmailEducacionalCommand(usuario));

           if(usuario.Email.NaoEstaPreenchido())
           {
               usuario.Email = usuario.EmailEducacional;
               await _mediator.Send(new AlterarEmailServicoAcessosCommand(usuario.Login, usuario.EmailEducacional));
           }
        }
        private async Task RemoverCache(string login)
        {
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.UsuarioLogado.Parametros(login)));
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Usuario.Parametros(login)));
        }
    }
}
