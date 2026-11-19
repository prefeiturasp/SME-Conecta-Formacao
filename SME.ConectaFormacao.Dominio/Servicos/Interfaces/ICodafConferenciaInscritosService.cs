using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface ICodafConferenciaInscritosService
    {
        Task SalvarInscritosAsync(List<CodafConferenciaInscricao> inscritos, long codafConferenciaId);
    }
}
