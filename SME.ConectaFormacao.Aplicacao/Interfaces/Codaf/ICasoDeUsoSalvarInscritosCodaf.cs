using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoSalvarInscritosCodaf
    {
        Task<Resultado> ExecutarAsync(IList<CodafInscritoListaPresencaSalvarDto> inscritos, long codafListaPresencaId);
    }
}
