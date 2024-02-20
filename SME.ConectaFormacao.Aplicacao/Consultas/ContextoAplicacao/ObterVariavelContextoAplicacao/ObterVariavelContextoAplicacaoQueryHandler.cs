using MediatR;
using SME.ConectaFormacao.Dominio.Contexto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterVariavelContextoAplicacaoQueryHandler : IRequestHandler<ObterVariavelContextoAplicacaoQuery, string>
    {
        private readonly IContextoAplicacao _contextoAplicacao;

        public ObterVariavelContextoAplicacaoQueryHandler(IContextoAplicacao contextoAplicacao)
        {
            _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));
        }

        public Task<string> Handle(ObterVariavelContextoAplicacaoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_contextoAplicacao.ObterVariavel<string>(request.Nome));
        }
    }
}
