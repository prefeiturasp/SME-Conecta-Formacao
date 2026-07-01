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
            var processador = new ProcessadorUploadAnexoTemporario(servicoArmazenamento);
            return await processador.ProcessarUploadAsync(arquivoDto);
        }
    }
}
