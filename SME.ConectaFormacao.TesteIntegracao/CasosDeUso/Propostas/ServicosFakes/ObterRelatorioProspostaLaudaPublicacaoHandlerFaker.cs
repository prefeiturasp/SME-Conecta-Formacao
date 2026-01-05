using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterRelatorioProspostaLaudaPublicacaoHandlerFaker : IRequestHandler<ObterRelatorioProspostaLaudaPublicacaoQuery, string>
    {
        public Task<string> Handle(ObterRelatorioProspostaLaudaPublicacaoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult("url relatorio");
        }
    }
}
