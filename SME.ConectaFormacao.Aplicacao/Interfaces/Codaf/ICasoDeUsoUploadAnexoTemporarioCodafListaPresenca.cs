using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca
    {
        Task<Resultado<CodafAnexoTemporarioDto>> ExecutarAsync(IFormFile arquivoDto);
    }
}