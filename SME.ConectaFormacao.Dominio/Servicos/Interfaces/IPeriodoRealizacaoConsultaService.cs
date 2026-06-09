
using SME.ConectaFormacao.Dominio.Entidades;

namespace ConectaFormacao.Dominio.Servicos
{
    public interface IPeriodoRealizacaoConsultaService
    {
        Task<PeriodoRealizacao?> ObterPeriodoRealizacaoAsync(long propostaTurmaId);
    }
}