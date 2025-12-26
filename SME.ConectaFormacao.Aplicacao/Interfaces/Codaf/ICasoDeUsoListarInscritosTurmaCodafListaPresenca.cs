using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoListarInscritosTurmaCodafListaPresenca
    {
        Task<Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>> ExecutarAsync(long propostaTurmaId, int numeroPagina = 1, int numeroRegistros = 10);
    }
}
