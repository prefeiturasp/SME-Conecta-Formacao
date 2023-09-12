using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeUsuarioLogadoQueryHandler : IRequestHandler<ObterNomeUsuarioLogadoQuery, string>
    {
        private readonly IContextoAplicacao _contextoAplicacao;

        public ObterNomeUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<string> Handle(ObterNomeUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_contextoAplicacao.NomeUsuario);
        }
    }
}
