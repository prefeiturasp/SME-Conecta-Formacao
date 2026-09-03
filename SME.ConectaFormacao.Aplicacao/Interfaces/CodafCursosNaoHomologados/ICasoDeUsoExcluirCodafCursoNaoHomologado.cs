using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados
{
    public interface ICasoDeUsoExcluirCodafCursoNaoHomologado
    {
        Task<Resultado> ExecutarAsync(long codafCursoNaoHomologadoId);
    }
}