using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes
{
    public interface ICasoDeUsoEmitirDeclaracaoCodaf
    {
        Task<Resultado> ExecutarAsync(long codafNaoHomologadoId);
    }
}
