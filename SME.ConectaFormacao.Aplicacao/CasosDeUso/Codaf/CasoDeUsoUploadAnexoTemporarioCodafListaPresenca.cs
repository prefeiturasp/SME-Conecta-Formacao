using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoUploadAnexoTemporarioCodafListaPresenca(IServicoArmazenamento servicoArmazenamento) : ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca
    {
        // 20MB - TODO: configurar no rancher (by Diego Moreno - 01/2026)
        private const long LIMITE_TAMANHO_BYTES = 20 * 1024 * 1024;
        public async Task<Resultado<CodafAnexoTemporarioDto>> ExecutarAsync(IFormFile arquivoDto)
        {
            var erroValidacao = ValidarRegrasDeNegocio(arquivoDto);
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

        private static Erro? ValidarRegrasDeNegocio(IFormFile arquivoDto)
        {
            if (arquivoDto.Length == 0)
                return Erro.Validacao("Nenhum arquivo foi enviado.");

            if (arquivoDto.Length > LIMITE_TAMANHO_BYTES)
                return Erro.Validacao($"O tamanho do arquivo excede o limite máximo de {LIMITE_TAMANHO_BYTES / (1024 * 1024)} MB.");
            var extensao = Path.GetExtension(arquivoDto.FileName).ToLower();
            if (extensao != ".pdf" || arquivoDto.ContentType != "application/pdf")
                return Erro.Validacao($"Extensão de arquivo '{extensao}' não é permitida. Extensões permitidas: .pdf");
            return null;
        }
    }
}
