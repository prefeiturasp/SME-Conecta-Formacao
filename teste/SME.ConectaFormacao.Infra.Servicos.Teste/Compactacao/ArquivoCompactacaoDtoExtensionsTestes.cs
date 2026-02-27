using FluentAssertions;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;
using System.Runtime.CompilerServices;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Compactacao
{
    public class ArquivoCompactacaoDtoExtensionsTestes
    {
        [Fact]
        public async Task DadoFluxoVazio_QuandoTratarNomesDuplicados_EntaoDeveRetornarVazio()
        {
            // Arrange
            var arquivosEntrada = GerarFluxoAssincrono(Enumerable.Empty<ArquivoCompactacaoDto>());

            // Act
            var resultado = new List<ArquivoCompactacaoDto>();
            await foreach (var arquivo in arquivosEntrada.TratarNomesDuplicados())
            {
                resultado.Add(arquivo);
            }

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoArquivosComNomesDiferentes_QuandoTratarNomesDuplicados_EntaoNaoDeveAlterarNomes()
        {
            // Arrange
            var entrada = new List<ArquivoCompactacaoDto>
            {
                new("CERTIFICADO_001.pdf", Stream.Null),
                new("CERTIFICADO_002.pdf", Stream.Null)
            };
            var arquivosEntrada = GerarFluxoAssincrono(entrada);

            // Act
            var resultado = new List<ArquivoCompactacaoDto>();
            await foreach (var arquivo in arquivosEntrada.TratarNomesDuplicados())
            {
                resultado.Add(arquivo);
            }

            // Assert
            resultado.Should().HaveCount(2);
            resultado[0].NomeArquivo.Should().Be("CERTIFICADO_001.pdf");
            resultado[1].NomeArquivo.Should().Be("CERTIFICADO_002.pdf");
        }

        [Fact]
        public async Task DadoArquivosComNomesIguais_QuandoTratarNomesDuplicados_EntaoDeveAdicionarContadorNoSufixo()
        {
            // Arrange
            var entrada = new List<ArquivoCompactacaoDto>
            {
                new("CERTIFICADO_001.pdf", Stream.Null),
                new("CERTIFICADO_001.pdf", Stream.Null),
                new("CERTIFICADO_001.pdf", Stream.Null)
            };
            var arquivosEntrada = GerarFluxoAssincrono(entrada);

            // Act
            var resultado = new List<ArquivoCompactacaoDto>();
            await foreach (var arquivo in arquivosEntrada.TratarNomesDuplicados())
            {
                resultado.Add(arquivo);
            }

            // Assert
            resultado.Should().HaveCount(3);
            resultado[0].NomeArquivo.Should().Be("CERTIFICADO_001.pdf");
            resultado[1].NomeArquivo.Should().Be("CERTIFICADO_001 (1).pdf");
            resultado[2].NomeArquivo.Should().Be("CERTIFICADO_001 (2).pdf");
        }

        [Fact]
        public async Task DadoArquivosComNomesIguaisEIgnorandoCase_QuandoTratarNomesDuplicados_EntaoDeveTratarComoDuplicado()
        {
            // Arrange
            var entrada = new List<ArquivoCompactacaoDto>
            {
                new("documento.txt", Stream.Null),
                new("DOCUMENTO.txt", Stream.Null)
            };
            var arquivosEntrada = GerarFluxoAssincrono(entrada);

            // Act
            var resultado = new List<ArquivoCompactacaoDto>();
            await foreach (var arquivo in arquivosEntrada.TratarNomesDuplicados())
            {
                resultado.Add(arquivo);
            }

            // Assert
            resultado.Should().HaveCount(2);
            resultado[0].NomeArquivo.Should().Be("documento.txt");
            resultado[1].NomeArquivo.Should().Be("DOCUMENTO (1).txt");
        }

        [Fact]
        public async Task DadoCancelamentoSolicitado_QuandoTratarNomesDuplicados_EntaoDeveLancarOperationCanceledException()
        {
            // Arrange
            var entrada = new List<ArquivoCompactacaoDto>
            {
                new("arquivo.pdf", Stream.Null),
                new("arquivo2.pdf", Stream.Null)
            };

            using var cts = new CancellationTokenSource();
            var arquivosEntrada = GerarFluxoAssincrono(entrada);

            // Act
            Func<Task> acao = async () =>
            {
                await foreach (var arquivo in arquivosEntrada.TratarNomesDuplicados(cts.Token))
                {
                    // Cancela a operação logo após receber o primeiro item
                    cts.Cancel();
                }
            };

            // Assert
            await acao.Should().ThrowAsync<OperationCanceledException>();
        }

        private async static IAsyncEnumerable<ArquivoCompactacaoDto> GerarFluxoAssincrono(
            IEnumerable<ArquivoCompactacaoDto> arquivos,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var arquivo in arquivos)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return arquivo;
                await Task.Yield();
            }
        }
    }
}