using SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces;
using System.IO.Compression;

namespace SME.ConectaFormacao.Infra.Servicos.Compactacao
{
    public class ServicoCompactacao : IServicoCompactacao
    {
        public async Task CompactarAssincronamenteAsync(
            IAsyncEnumerable<ArquivoCompactacaoDto> arquivos,
            Stream streamSaida,
            CancellationToken cancellationToken = default)
        {
            using var arquivoCompactacao = new ZipArchive(streamSaida, ZipArchiveMode.Create, true);

            await foreach (var arquivo in arquivos.WithCancellation(cancellationToken))
            {
                var entradaZip = arquivoCompactacao.CreateEntry(arquivo.NomeArquivo, CompressionLevel.Optimal);

                using var entradaStream = await entradaZip.OpenAsync(cancellationToken);

                if (arquivo.Conteudo.CanSeek)
                    arquivo.Conteudo.Position = 0;

                await arquivo.Conteudo.CopyToAsync(entradaStream, cancellationToken);
                await arquivo.Conteudo.DisposeAsync();
            }
        }
    }
}
