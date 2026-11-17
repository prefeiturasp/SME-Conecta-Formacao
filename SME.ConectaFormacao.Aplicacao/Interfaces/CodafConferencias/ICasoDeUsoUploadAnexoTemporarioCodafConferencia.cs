using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoUploadAnexoTemporarioCodafConferencia
    {
        Task<Resultado<CodafAnexoTemporarioDto>> ExecutarAsync(IFormFile arquivoDto);
    }
}
