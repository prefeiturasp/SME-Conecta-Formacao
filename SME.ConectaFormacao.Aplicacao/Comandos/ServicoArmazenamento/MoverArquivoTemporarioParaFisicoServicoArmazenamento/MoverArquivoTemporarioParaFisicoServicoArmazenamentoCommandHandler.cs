using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommandHandler : IRequestHandler<MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand, string>
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommandHandler(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }

        public Task<string> Handle(MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            return _servicoArmazenamento.Mover(request.Nome);
        }
    }
}
