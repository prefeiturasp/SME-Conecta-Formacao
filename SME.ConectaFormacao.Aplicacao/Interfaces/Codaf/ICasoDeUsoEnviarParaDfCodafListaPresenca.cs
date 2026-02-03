using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoEnviarParaDfCodafListaPresenca
    {
        Task<Resultado<bool>> ExecutarAsync(long codafListaPresencaId);
    }
}