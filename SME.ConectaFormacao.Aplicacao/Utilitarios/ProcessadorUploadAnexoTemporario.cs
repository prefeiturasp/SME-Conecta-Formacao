using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Utilitarios
{
    public class ProcessadorUploadAnexoTemporario
    {
        private readonly IServicoArmazenamento _servicoArmazenamento;

        public ProcessadorUploadAnexoTemporario(IServicoArmazenamento servicoArmazenamento)
        {
            _servicoArmazenamento = servicoArmazenamento;
        }

        public async Task<Resultado<CodafAnexoTemporarioDto>> ProcessarUploadAsync(IFormFile arquivoDto)
        {
            var erroValidacao = ValidadorArquivoUploadTemporario.ValidarArquivoPdf(arquivoDto);
            if (erroValidacao != null)
                return erroValidacao;

            using var stream = arquivoDto.OpenReadStream();
            var arquivoId = await _servicoArmazenamento.ArmazenarTemporariaGuid(stream, arquivoDto.ContentType);

            if (arquivoId == Guid.Empty)
                return Resultado<CodafAnexoTemporarioDto>.DeFalha(TipoFalha.ErroInterno, "Erro ao salvar anexo, tente novamente.");

            return new CodafAnexoTemporarioDto
            {
                ArquivoCodigo = arquivoId,
                NomeArquivo = arquivoDto.FileName,
                Extensao = Path.GetExtension(arquivoDto.FileName),
                UrlDownload = await _servicoArmazenamento.ObterUrlPorChaveObjetoAsync(arquivoId.ToString(), true),
                ContentType = arquivoDto.ContentType,
                TamanhoBytes = arquivoDto.Length
            };
        }
    }
}