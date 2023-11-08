using MediatR;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioLogadoQueryHandler :IRequestHandler<ObterUsuarioLogadoQuery,Usuario>
    {
        private readonly IContextoAplicacao _contextoAplicacao;
        private readonly IMediator _mediator;

        public ObterUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao,IMediator mediator)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new ObterUsuarioPorLoginQuery(_contextoAplicacao.UsuarioLogado), cancellationToken);
        }
    }
}