using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ArmazenarArquivoFisicoServicoArmazenamentoCommandHandler : IRequestHandler<ArmazenarArquivoFisicoServicoArmazenamentoCommand, string>
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public ArmazenarArquivoFisicoServicoArmazenamentoCommandHandler(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento ?? throw new ArgumentNullException(nameof(servicoArmazenamento));
        }

        public async Task<string> Handle(ArmazenarArquivoFisicoServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            var nomeArquivo = $"{request.Arquivo.Codigo}{Path.GetExtension(request.Arquivo.FormFile.FileName)}";

            var stream = request.Arquivo.FormFile.OpenReadStream();
            return await _servicoArmazenamento.Armazenar(nomeArquivo, stream, request.Arquivo.TipoConteudo);
        }
    }
}
