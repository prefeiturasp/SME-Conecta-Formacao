using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo
{
    public interface ICasoDeUsoArquivoCarregarTemporario
    {
        Task<ArquivoArmazenadoDTO> Executar(IFormFile file);
    }
}
