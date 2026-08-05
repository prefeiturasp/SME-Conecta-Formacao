using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoObterCodafCursoNaoHomologadoPorId
    {
        Task<Resultado<CodafCursoNaoHomologadoDetalhadoDto>> ExecutarAsync(long codafCursoNaoHomologadoId);
    }
}