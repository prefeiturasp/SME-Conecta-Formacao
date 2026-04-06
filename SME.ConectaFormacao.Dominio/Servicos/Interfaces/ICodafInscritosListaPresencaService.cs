using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface ICodafInscritosListaPresencaService
    {
        Task SalvarInscritosAsync(List<CodafInscricaoListaPresenca> inscritos, long codafListaPresencaId);
    }
}
