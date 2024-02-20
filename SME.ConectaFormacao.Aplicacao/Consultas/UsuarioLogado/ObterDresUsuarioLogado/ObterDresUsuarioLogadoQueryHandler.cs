using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDresUsuarioLogadoQueryHandler : IRequestHandler<ObterDresUsuarioLogadoQuery, string[]>
    {
        private readonly IContextoAplicacao contextoAplicacao;

        public ObterDresUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            this.contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<string[]> Handle(ObterDresUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(contextoAplicacao.ObterVariavel<string[]>("Dres"));
        }
    }
}
