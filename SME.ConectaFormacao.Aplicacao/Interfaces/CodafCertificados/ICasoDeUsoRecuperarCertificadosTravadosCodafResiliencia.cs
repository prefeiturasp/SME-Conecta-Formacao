namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoRecuperarCertificadosTravadosCodafResiliencia
    {
        Task ExecutarAsync(CancellationToken cancellationToken);
    }
}
