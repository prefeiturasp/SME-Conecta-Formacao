using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoTurmaPossuiCodafConferencia(
        IRepositorioCodafConferencia repositorioCodafConferencia) : ICasoDeUsoTurmaPossuiCodafConferencia
    {
        public async Task<Resultado<bool>> ExecutarAsync(long propostaTurmaId, long? codafConferenciaId = null)
        {
            var possui = await repositorioCodafConferencia.TurmaJaTemCodafAsync(propostaTurmaId, codafConferenciaId);
            return Resultado<bool>.DeSucesso(possui);
        }
    }
}
