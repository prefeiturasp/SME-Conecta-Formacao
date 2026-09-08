using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoFinalizarCodafCursoNaoHomologado
    {
        Task<Resultado> ExecutarAsync(long codafCursoNaoHomologadoId, FinalizarCodafDto dto);
    }
}
