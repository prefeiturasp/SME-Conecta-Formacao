using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoUploadAnexoTemporarioCodafSuplementar(IServicoArmazenamento servicoArmazenamento) : ICasoDeUsoUploadAnexoTemporarioCodafSuplementar
    {
        public async Task<Resultado<CodafAnexoTemporarioDto>> ExecutarAsync(IFormFile arquivoDto)
        {
            var erroValidacao = ValidadorArquivoUploadTemporario.ValidarArquivoPdf(arquivoDto);
            if (erroValidacao != null)
                return erroValidacao;

            Guid arquivoId;

            using var stream = arquivoDto.OpenReadStream();
            arquivoId = await servicoArmazenamento.ArmazenarTemporariaGuid(stream, arquivoDto.ContentType);

            return new CodafAnexoTemporarioDto
            {
                ArquivoCodigo = arquivoId,
                NomeArquivo = arquivoDto.FileName,
                Extensao = Path.GetExtension(arquivoDto.FileName),
                UrlDownload = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(arquivoId.ToString(), true),
                ContentType = arquivoDto.ContentType,
                TamanhoBytes = arquivoDto.Length
            };
        }
    }
}
