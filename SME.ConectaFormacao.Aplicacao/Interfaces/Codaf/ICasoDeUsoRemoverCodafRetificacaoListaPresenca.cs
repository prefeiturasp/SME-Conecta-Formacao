using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoRemoverCodafRetificacaoListaPresenca
    {
        Task<Resultado<bool>> ExecutarAsync(long codafRetificacaoListaPresencaId);
    }
}