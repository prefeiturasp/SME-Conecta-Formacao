using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoUploadAnexoTemporarioCodafConferencia(IServicoArmazenamento servicoArmazenamento) : ICasoDeUsoUploadAnexoTemporarioCodafConferencia
    {
        public async Task<Resultado<CodafAnexoTemporarioDto>> ExecutarAsync(IFormFile arquivoDto)
        {
            if (arquivoDto is null)
                return Erro.Validacao("Nenhum arquivo foi enviado.");

            var processador = new ProcessadorUploadAnexoTemporario(servicoArmazenamento);
            return await processador.ProcessarUploadAsync(arquivoDto);
        }
    }
}
