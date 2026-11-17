using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias
{
    public interface ICasoDeUsoListarInscritosTurmaCodafConferencia
    {
        Task<Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>> ExecutarAsync(
            long propostaTurmaId,
            int numeroPagina = 1,
            int numeroRegistros = 10,
            long? codafConferenciaId = null);
    }
}
