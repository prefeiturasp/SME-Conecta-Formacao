namespace SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes
{
    public interface ICasoDeUsoDownloadLoteDeclaracoes
    {
        Task ExecutarAsync(List<long> ids, Stream streamSaida, CancellationToken cancellationToken = default);
    }
}
