using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoObterPropostaTurmaComCodaf
    {
        Task<Resultado<IEnumerable<PropostaTurmaComCodafDto>>> ExecutarAsync(long propostaId);
    }
}
