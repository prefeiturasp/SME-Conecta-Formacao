namespace SME.ConectaFormacao.Aplicacao.Interfaces.Codaf
{
    public interface ICasoDeUsoRecuperarCertificadosTravadosCodafResiliencia
    {
        Task ExecutarAsync(CancellationToken cancellationToken);
    }
}
