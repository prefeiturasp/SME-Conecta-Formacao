using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterRelatorioProspostaLaudaCompletoHandlerFaker : IRequestHandler<ObterRelatorioProspostaLaudaCompletaQuery, string>
    {
        public Task<string> Handle(ObterRelatorioProspostaLaudaCompletaQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult("url relatorio lauda completo");
        }
    }
}
