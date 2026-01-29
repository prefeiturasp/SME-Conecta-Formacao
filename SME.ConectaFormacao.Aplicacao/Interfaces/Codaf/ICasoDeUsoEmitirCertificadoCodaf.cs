namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoEmitirCertificadoCodaf
    {
        Task<bool> ExecutarAsync(long codafListaPresencaId);
    }
}
