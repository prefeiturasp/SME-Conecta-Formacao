using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(IServicoArmazenamento servicoArmazenamento) : 
        IRequestHandler<ArmazenarArquivoTemporarioServicoArmazenamentoCommand, string>
    {
        public async Task<string> Handle(ArmazenarArquivoTemporarioServicoArmazenamentoCommand request, CancellationToken cancellationToken)
        {
            var nomeArquivo = $"{request.Arquivo.Codigo}{Path.GetExtension(request.Arquivo.FormFile.FileName)}";

            var stream = request.Arquivo.FormFile.OpenReadStream();
            return await servicoArmazenamento.ArmazenarTemporaria(nomeArquivo, stream, request.Arquivo.TipoConteudo);
        }
    }
}