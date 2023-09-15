using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommandHandlerFaker : IRequestHandler<MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand, string>
    {
        public Task<string> Handle(MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }
    }
}
