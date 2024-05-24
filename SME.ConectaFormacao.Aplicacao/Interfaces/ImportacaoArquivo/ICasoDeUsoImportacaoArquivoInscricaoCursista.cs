using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo
{
    public interface ICasoDeUsoImportacaoArquivoInscricaoCursista
    {
        Task<RetornoDTO> Executar(IFormFile arquivo, long propostaId);
    }
}
