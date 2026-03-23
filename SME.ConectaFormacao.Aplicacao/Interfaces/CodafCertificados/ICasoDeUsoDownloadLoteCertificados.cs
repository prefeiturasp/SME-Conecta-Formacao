namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados
{
    public interface ICasoDeUsoDownloadLoteCertificados
    {
        Task ExecutarAsync(List<long> ids, Stream streamSaida, CancellationToken cancellationToken = default);
    }
}
