using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    public class RemoverArquivoServicoArmazenamentoCommandHandlerFake : IRequestHandler<RemoverArquivoServicoArmazenamentoCommand, bool>
    {
        public Task<bool> Handle(RemoverArquivoServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
