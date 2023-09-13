using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEmailUsuarioLogadoQueryHandler : IRequestHandler<ObterEmailUsuarioLogadoQuery, string>
    {
        private readonly IMediator _mediator;
        private readonly IContextoAplicacao _contextoAplicacao;

        public ObterEmailUsuarioLogadoQueryHandler(IMediator mediator, IContextoAplicacao contextoAplicacao)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public async Task<string> Handle(ObterEmailUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(_contextoAplicacao.UsuarioLogado), cancellationToken);
            return usuario.Email;
        }
    }
}
