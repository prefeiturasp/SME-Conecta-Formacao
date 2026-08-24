using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes
{
    public interface ICasoDeUsoObterDeclaracaoCodafParaDownload
    {
        Task<Resultado<CodafDeclaracaoParaDownloadDto>> ExecutarAsync(long declaracaoCodafId);
    }
}