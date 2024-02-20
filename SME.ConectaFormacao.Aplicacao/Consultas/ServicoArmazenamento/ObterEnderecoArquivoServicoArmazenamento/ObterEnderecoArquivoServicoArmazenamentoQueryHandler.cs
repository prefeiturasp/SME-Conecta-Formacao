using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEnderecoArquivoServicoArmazenamentoQueryHandler : IRequestHandler<ObterEnderecoArquivoServicoArmazenamentoQuery, string>
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public ObterEnderecoArquivoServicoArmazenamentoQueryHandler(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }

        public Task<string> Handle(ObterEnderecoArquivoServicoArmazenamentoQuery request, CancellationToken cancellationToken)
        {
            return _servicoArmazenamento.Obter(request.NomeArquivoFisico, request.EhTemp);
        }
    }
}
