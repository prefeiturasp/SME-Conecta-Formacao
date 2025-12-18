using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoAtualizarCodafListaPresenca
    {
        Task<Resultado> ExecutarAsync(CodafListaPresencaEdicaoDto codafListaPresencaEdicaoDto, long id);
    }
}
