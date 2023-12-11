using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.ServicosFakes
{
    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandlerFake : IRequestHandler<ArmazenarArquivoTemporarioServicoArmazenamentoCommand, string>
    {
        public Task<string> Handle(ArmazenarArquivoTemporarioServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
