using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Dominio.Servicos.Interfaces
{
    public interface ICodafCursoNaoHomologadoInscritosService
    {
        Task SalvarInscritosAsync(List<CodafCursoNaoHomologadoInscricao> inscritos, long codafCursoNaoHomologadoId);
    }
}