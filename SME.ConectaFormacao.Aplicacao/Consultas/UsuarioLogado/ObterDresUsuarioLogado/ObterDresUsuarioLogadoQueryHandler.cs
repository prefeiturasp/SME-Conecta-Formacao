using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDresUsuarioLogadoQueryHandler : IRequestHandler<ObterDresUsuarioLogadoQuery, IEnumerable<string>>
    {
        private readonly IContextoAplicacao _contextoAplicacao;

        public ObterDresUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<IEnumerable<string>> Handle(ObterDresUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_contextoAplicacao.Dres);
        }
    }
}
