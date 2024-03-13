using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo
{
    public interface ICasoDeUsoImportacaoArquivoInscricaoCursista
    {
        Task<RetornoDTO> Executar(ImportacaoArquivoInscricaoDTO inscricao);
    }
}
