using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPeriodoRealizacaoConsulta
    {
        Task<PeriodoRealizacao?> ObterPeriodoRealizacaoAsync(long propostaTurmaId);
    }
}