using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca
    {
        Task<Resultado<bool>> ExecutarAsync(long codafListaPresencaId, string justificativa);
    }
}
