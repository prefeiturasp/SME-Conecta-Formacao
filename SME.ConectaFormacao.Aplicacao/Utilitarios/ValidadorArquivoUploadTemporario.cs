using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public static class ValidadorArquivoUploadTemporario
    {
        // 20MB - TODO: configurar no rancher (by Diego Moreno - 01/2026)
        private const long LIMITE_TAMANHO_BYTES = 20 * 1024 * 1024;

        public static Erro? ValidarArquivoPdf(IFormFile arquivo)
        {
            if (arquivo.Length == 0)
                return Erro.Validacao("Nenhum arquivo foi enviado.");

            if (arquivo.Length > LIMITE_TAMANHO_BYTES)
                return Erro.Validacao($"O tamanho do arquivo excede o limite máximo de {LIMITE_TAMANHO_BYTES / (1024 * 1024)} MB.");

            var extensao = Path.GetExtension(arquivo.FileName).ToLower();
            if (extensao != ".pdf" || arquivo.ContentType != "application/pdf")
                return Erro.Validacao($"Extensão de arquivo '{extensao}' não é permitida. Extensões permitidas: .pdf");

            return null;
        }
    }
}