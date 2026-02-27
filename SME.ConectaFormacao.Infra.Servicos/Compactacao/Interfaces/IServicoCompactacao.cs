namespace SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces
{
    public interface IServicoCompactacao
    {
        Task CompactarAssincronamenteAsync(
            IAsyncEnumerable<ArquivoCompactacaoDto> arquivos,
            Stream streamSaida,
            CancellationToken cancellationToken = default);
    }
}