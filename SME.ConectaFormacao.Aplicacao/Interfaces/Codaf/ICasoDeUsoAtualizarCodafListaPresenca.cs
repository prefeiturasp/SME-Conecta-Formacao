using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoAtualizarCodafListaPresenca
    {
        Task<Resultado<CodafListaPresencaDto>> ExecutarAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, int id);
    }
}
