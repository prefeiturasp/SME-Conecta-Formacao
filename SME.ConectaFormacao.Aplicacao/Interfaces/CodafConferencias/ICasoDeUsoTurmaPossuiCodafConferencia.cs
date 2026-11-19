using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoTurmaPossuiCodafConferencia
    {
        Task<Resultado<bool>> ExecutarAsync(long propostaTurmaId, long? codafConferenciaId = null);
    }
}
