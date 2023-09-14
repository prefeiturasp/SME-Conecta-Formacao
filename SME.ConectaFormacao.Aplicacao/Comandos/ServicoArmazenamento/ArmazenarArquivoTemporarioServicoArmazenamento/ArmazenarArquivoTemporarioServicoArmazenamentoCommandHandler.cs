using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler : IRequestHandler<ArmazenarArquivoTemporarioServicoArmazenamentoCommand, string>
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }

        public async Task<string> Handle(ArmazenarArquivoTemporarioServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            var nomeArquivo = $"{request.Arquivo.Codigo}{Path.GetExtension(request.Arquivo.FormFile.FileName)}";

            var stream = request.Arquivo.FormFile.OpenReadStream();
            return await _servicoArmazenamento.ArmazenarTemporaria(nomeArquivo, stream, request.Arquivo.TipoConteudo);
        }
    }
}
