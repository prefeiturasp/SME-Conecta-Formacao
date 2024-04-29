using MediatR;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPermissaoParecerPerfilLogadoQueryHandler : IRequestHandler<ObterPermissaoParecerPerfilLogadoQuery, bool>
    {
        private readonly IMediator _mediator;

        public ObterPermissaoParecerPerfilLogadoQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(ObterPermissaoParecerPerfilLogadoQuery request, CancellationToken cancellationToken)
        {
            var perfilLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery(), cancellationToken);

            return perfilLogado.EhPerfilParecerista() ||
                perfilLogado.EhPerfilAdminDF() ||
                (await _mediator.Send(new ObterPerfilAreaPromotoraQuery(perfilLogado), cancellationToken)).NaoEhNulo();
        }
    }
}
