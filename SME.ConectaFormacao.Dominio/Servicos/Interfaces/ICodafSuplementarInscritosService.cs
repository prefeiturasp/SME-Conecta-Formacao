using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface ICodafSuplementarInscritosService
    {
        Task SalvarInscritosAsync(List<CodafSuplementarInscricao> inscritos, long codafSuplementarId);
    }
}
