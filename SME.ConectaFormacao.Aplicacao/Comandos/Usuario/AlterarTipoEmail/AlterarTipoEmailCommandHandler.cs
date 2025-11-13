using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text.RegularExpressions;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Usuario.AlterarEmailEducacional
{
    public class AlterarTipoEmailCommandHandler : IRequestHandler<AlterarTipoEmailCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IMediator _mediator;

        public AlterarTipoEmailCommandHandler(IRepositorioUsuario repositorioUsuario, IMediator mediator)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AlterarTipoEmailCommand request, CancellationToken cancellationToken)
        {
            var realizouAtualizacao = await _repositorioUsuario.AtualizarTipoEmail(request.Login, request.Tipo);
            if (!realizouAtualizacao)
                throw new NegocioException(MensagemNegocio.TIPO_EMAIL_NAO_ATUALIZADO);

            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.UsuarioLogado.Parametros(request.Login)));
            await _mediator.Send(new RemoverCacheCommand(CacheDistribuidoNomes.Usuario.Parametros(request.Login)));

            return realizouAtualizacao;
        }
    }
}