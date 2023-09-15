using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    public class ObterEnderecoArquivoServicoArmazenamentoQueryHandlerFake : IRequestHandler<ObterEnderecoArquivoServicoArmazenamentoQuery, string>
    {
        public Task<string> Handle(ObterEnderecoArquivoServicoArmazenamentoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(BaixarArquivoMock.GerarUrlImagem());
        }
    }
}
