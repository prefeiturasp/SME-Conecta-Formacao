using FluentAssertions;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Compactacao
{
    public class ServicoCompactacaoTests
    {
        private readonly ServicoCompactacao _sut;

        public ServicoCompactacaoTests()
        {
            _sut = new ServicoCompactacao();
        }

        [Fact]
        public async Task DadoFluxoDeArquivosValidos_QuandoCompactarAssincronamenteAsync_EntaoDeveGerarArquivoZipValido()
        {
            // Arrange
            using var streamSaida = new MemoryStream();
            var arquivosEntrada = GerarFluxoArquivosFalsosAsync();

            // Act
            await _sut.CompactarAssincronamenteAsync(arquivosEntrada, streamSaida);

            // Assert
            streamSaida.Length.Should().BeGreaterThan(0);
            streamSaida.Position = 0;

            using var zipArchive = new ZipArchive(streamSaida, ZipArchiveMode.Read);

            zipArchive.Entries.Should().HaveCount(2, "Porque foram enviados 2 arquivos no IAsyncEnumerable");

            var primeiroArquivo = zipArchive.GetEntry("CERTIFICADO_001.pdf");
            primeiroArquivo.Should().NotBeNull();
            primeiroArquivo!.Length.Should().BeGreaterThan(0);

            var segundoArquivo = zipArchive.GetEntry("ERRO_002.txt");
            segundoArquivo.Should().NotBeNull();
            segundoArquivo!.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task DadoCancelamentoSolicitado_QuandoCompactarAssincronamenteAsync_EntaoDeveLancarOperationCanceledException()
        {
            // Arrange
            using var streamSaida = new MemoryStream();
            var arquivosEntrada = GerarFluxoArquivosFalsosAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // Cancela imediatamente antes de começar

            // Act
            Func<Task> acao = async () =>
                await _sut.CompactarAssincronamenteAsync(arquivosEntrada, streamSaida, cancellationTokenSource.Token);

            // Assert
            await acao.Should().ThrowAsync<OperationCanceledException>();
        }

        private async static IAsyncEnumerable<ArquivoCompactacaoDto> GerarFluxoArquivosFalsosAsync([EnumeratorCancellation] CancellationToken ct = default)
        {
            var bytesFalsosPdf = Encoding.UTF8.GetBytes("Isso é um PDF falso");
            var streamPdf = new MemoryStream(bytesFalsosPdf);
            yield return new ArquivoCompactacaoDto("CERTIFICADO_001.pdf", streamPdf);

            await Task.Delay(10, ct);

            var bytesFalsosTxt = Encoding.UTF8.GetBytes("Isso é um erro falso");
            var streamTxt = new MemoryStream(bytesFalsosTxt);
            yield return new ArquivoCompactacaoDto("ERRO_002.txt", streamTxt);
        }
    }
}
