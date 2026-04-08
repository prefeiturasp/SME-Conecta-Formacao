using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface IPropostaGrupoPeriodoService
    {
        Task ProcessarGruposAsync(long propostaId, IEnumerable<PropostaGrupoPeriodo> grupos);
    }
}
