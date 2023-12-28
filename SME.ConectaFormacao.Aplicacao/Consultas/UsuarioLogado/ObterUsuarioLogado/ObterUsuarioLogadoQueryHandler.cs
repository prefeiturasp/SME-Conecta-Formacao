using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioLogadoQueryHandler : IRequestHandler<ObterUsuarioLogadoQuery, Usuario>
    {
        private readonly IContextoAplicacao _contextoAplicacao;
        private readonly IMediator _mediator;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao, IMediator mediator, ICacheDistribuido cacheDistribuido)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            var nomeChave = CacheDistribuidoNomes.UsuarioLogado.Parametros(_contextoAplicacao.UsuarioLogado);

            return await _cacheDistribuido.ObterAsync(nomeChave, () => _mediator.Send(new ObterUsuarioPorLoginQuery(_contextoAplicacao.UsuarioLogado), cancellationToken));
        }
    }
}