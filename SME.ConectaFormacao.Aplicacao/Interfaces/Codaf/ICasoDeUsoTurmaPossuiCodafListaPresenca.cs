using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoTurmaPossuiCodafListaPresenca
    {
        Task<Resultado<bool>> ExecutarAsync(long propostaTurmaId, long listaPresencaId = 0);
    }
}
