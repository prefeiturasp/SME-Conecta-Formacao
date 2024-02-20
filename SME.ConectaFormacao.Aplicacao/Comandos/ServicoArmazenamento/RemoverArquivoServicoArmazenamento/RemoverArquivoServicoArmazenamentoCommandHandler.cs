using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivoServicoArmazenamentoCommandHandler : IRequestHandler<RemoverArquivoServicoArmazenamentoCommand, bool>
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public RemoverArquivoServicoArmazenamentoCommandHandler(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }

        public async Task<bool> Handle(RemoverArquivoServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            return await _servicoArmazenamento.Excluir(request.Nome);
        }
    }
}
