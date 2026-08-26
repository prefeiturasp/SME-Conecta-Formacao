using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;
using SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoDownloadLoteDeclaracoesTestes
    {
        private readonly Mock<IRepositorioCodafDeclaracao> repositorio;
        private readonly Mock<IServicoArmazenamento> servicoArmazenamento;
        private readonly Mock<IServicoCompactacao> servicoCompactacao;
        private readonly Mock<IServicoRelatorio> servicoRelatorio;

        public CasoDeUsoDownloadLoteDeclaracoesTestes()
        {
            repositorio = new Mock<IRepositorioCodafDeclaracao>();
            servicoArmazenamento = new Mock<IServicoArmazenamento>();
            servicoCompactacao = new Mock<IServicoCompactacao>();
            servicoRelatorio = new Mock<IServicoRelatorio>();
        }

        [Fact]
        public async Task ExecutarAsync_DeveLancarArgumentException_QuandoIdsForNulo()
        {
            // Arrange
            var sut = CriarSut();
            await using var streamSaida = new MemoryStream();

            // Act
            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => sut.ExecutarAsync(null!, streamSaida));

            // Assert
            Assert.Equal("ids", excecao.ParamName);
            Assert.Contains(
                "A lista de IDs não pode ser vazia.",
                excecao.Message);

            repositorio.Verify(
                x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(
                    It.IsAny<List<long>>()),
                Times.Never);

            servicoCompactacao.Verify(
                x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveLancarArgumentException_QuandoIdsForVazio()
        {
            // Arrange
            var sut = CriarSut();
            await using var streamSaida = new MemoryStream();

            // Act
            var excecao = await Assert.ThrowsAsync<ArgumentException>(
                () => sut.ExecutarAsync([], streamSaida));

            // Assert
            Assert.Equal("ids", excecao.ParamName);
            Assert.Contains(
                "A lista de IDs não pode ser vazia.",
                excecao.Message);

            repositorio.Verify(
                x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(
                    It.IsAny<List<long>>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveLancarInvalidOperationException_QuandoNaoExistiremDeclaracoes()
        {
            // Arrange
            var ids = new List<long> { 1, 2, 3 };

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao>());

            var sut = CriarSut();
            await using var streamSaida = new MemoryStream();

            // Act
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => sut.ExecutarAsync(ids, streamSaida));

            // Assert
            Assert.Equal(
                "Nenhuma declaração encontrada para os IDs informados.",
                excecao.Message);

            repositorio.Verify(
                x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids),
                Times.Once);

            servicoCompactacao.Verify(
                x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    It.IsAny<Stream>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveUsarArquivoDoStorage_QuandoArquivoExistir()
        {
            // Arrange
            const long codigoDeclaracao = 7;
            const string chave = "declaracoes/0007.pdf";

            var declaracao = CriarDeclaracao(
                codigoDeclaracao,
                chaveObjetoArmazenamento: chave);

            var ids = new List<long> { 1 };
            var streamStorage = new MemoryStream([1, 2, 3, 4]);
            var arquivosCompactados = new List<ArquivoCompactacaoDto>();

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao> { declaracao });

            servicoArmazenamento
                .Setup(x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Resultado<Stream>.DeSucesso(streamStorage));

            await using var streamSaida = new MemoryStream();

            ConfigurarCompactadorConsumindoArquivos(
                streamSaida,
                arquivosCompactados);

            var sut = CriarSut();

            // Act
            await sut.ExecutarAsync(ids, streamSaida);

            // Assert
            var arquivo = Assert.Single(arquivosCompactados);

            Assert.Equal(
                $"DECLARACAO_{codigoDeclaracao:D4}_{declaracao.DataEmissao:ddMMyyyy}.pdf",
                arquivo.NomeArquivo);

            Assert.Same(streamStorage, arquivo.Conteudo);

            servicoArmazenamento.Verify(
                x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            servicoCompactacao.Verify(
                x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    streamSaida,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveGerarPdf_QuandoArquivoNaoExistirNoStorage()
        {
            // Arrange
            const string chave = "declaracoes/inexistente.pdf";
            const string html = "<html><body>Declaração</body></html>";

            var declaracao = CriarDeclaracao(
                codigoDeclaracao: 15,
                chaveObjetoArmazenamento: chave,
                htmlContent: html);

            var ids = new List<long> { 15 };
            var streamPdf = new MemoryStream([10, 20, 30]);
            var arquivosCompactados = new List<ArquivoCompactacaoDto>();

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao> { declaracao });

            servicoArmazenamento
                .Setup(x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Resultado<Stream>.DeFalha(
                        default,
                        "Arquivo não encontrado"));

            servicoRelatorio
                .Setup(x => x.ConveterHtmlCodafParaPdfAsync(
                    It.Is<HtmlCodafDto>(dto =>
                        dto.HtmlContent == html),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Resultado<Stream>.DeSucesso(streamPdf));

            await using var streamSaida = new MemoryStream();

            ConfigurarCompactadorConsumindoArquivos(
                streamSaida,
                arquivosCompactados);

            var sut = CriarSut();

            // Act
            await sut.ExecutarAsync(ids, streamSaida);

            // Assert
            var arquivo = Assert.Single(arquivosCompactados);

            Assert.Equal(
                $"DECLARACAO_0015_{declaracao.DataEmissao:ddMMyyyy}.pdf",
                arquivo.NomeArquivo);

            Assert.Same(streamPdf, arquivo.Conteudo);

            servicoArmazenamento.Verify(
                x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.Is<HtmlCodafDto>(dto =>
                        dto.HtmlContent == html),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveGerarPdfDiretamente_QuandoNaoHouverChaveNoStorage()
        {
            // Arrange
            const string html = "<html>snapshot</html>";

            var declaracao = CriarDeclaracao(
                codigoDeclaracao: 21,
                chaveObjetoArmazenamento: null,
                htmlContent: html);

            var ids = new List<long> { 21 };
            var streamPdf = new MemoryStream([50, 60]);
            var arquivosCompactados = new List<ArquivoCompactacaoDto>();

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao> { declaracao });

            servicoRelatorio
                .Setup(x => x.ConveterHtmlCodafParaPdfAsync(
                    It.Is<HtmlCodafDto>(dto =>
                        dto.HtmlContent == html),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Resultado<Stream>.DeSucesso(streamPdf));

            await using var streamSaida = new MemoryStream();

            ConfigurarCompactadorConsumindoArquivos(
                streamSaida,
                arquivosCompactados);

            var sut = CriarSut();

            // Act
            await sut.ExecutarAsync(ids, streamSaida);

            // Assert
            var arquivo = Assert.Single(arquivosCompactados);

            Assert.Equal(
                $"DECLARACAO_0021_{declaracao.DataEmissao:ddMMyyyy}.pdf",
                arquivo.NomeArquivo);

            Assert.Same(streamPdf, arquivo.Conteudo);

            servicoArmazenamento.Verify(
                x => x.ObterArquivoPorChaveAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.Is<HtmlCodafDto>(dto =>
                        dto.HtmlContent == html),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveAdicionarTxtDeErro_QuandoGeracaoDoPdfFalhar()
        {
            // Arrange
            const long codigoDeclaracao = 32;

            var declaracao = CriarDeclaracao(
                codigoDeclaracao,
                chaveObjetoArmazenamento: null);

            var ids = new List<long> { codigoDeclaracao };
            var arquivosCompactados = new List<ArquivoCompactacaoDto>();

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao> { declaracao });

            servicoRelatorio
                .Setup(x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Resultado<Stream>.DeFalha(
                        default,
                        new List<string>
                        {
                            "Falha ao converter HTML",
                            "Serviço de relatório indisponível"
                        }));

            await using var streamSaida = new MemoryStream();

            ConfigurarCompactadorConsumindoArquivos(
                streamSaida,
                arquivosCompactados);

            var sut = CriarSut();

            // Act
            await sut.ExecutarAsync(ids, streamSaida);

            // Assert
            var arquivo = Assert.Single(arquivosCompactados);

            Assert.Equal(
                $"ERRO_DECLARACAO_{codigoDeclaracao:D4}_{declaracao.DataEmissao:ddMMyyyy}.txt",
                arquivo.NomeArquivo);

            arquivo.Conteudo.Position = 0;

            using var reader = new StreamReader(
                arquivo.Conteudo,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: true);

            var conteudo = await reader.ReadToEndAsync();

            Assert.Equal(
                """
                Erro ao gerar declaração N° 0032

                ERRO:
                Falha ao converter HTML
                 - Serviço de relatório indisponível
                """.Replace("\r\n", "\n"),
                conteudo.Replace("\r\n", "\n"));

            servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DevePropagarFalhaDoProducer_QuandoProcessamentoLancarExcecao()
        {
            // Arrange
            const string chave = "arquivo-com-falha.pdf";

            var declaracao = CriarDeclaracao(
                codigoDeclaracao: 40,
                chaveObjetoArmazenamento: chave);

            var ids = new List<long> { 40 };

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao> { declaracao });

            servicoArmazenamento
                .Setup(x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(
                    new InvalidOperationException(
                        "Erro inesperado no storage"));

            await using var streamSaida = new MemoryStream();

            /*
             * É importante consumir o IAsyncEnumerable.
             *
             * Se o mock simplesmente retornar Task.CompletedTask sem ler
             * o channel, o erro gravado em canal.Writer.Complete(ex)
             * pode não ser observado pelo teste.
             */
            servicoCompactacao
                .Setup(x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    streamSaida,
                    It.IsAny<CancellationToken>()))
                .Returns(
                    async (
                        IAsyncEnumerable<ArquivoCompactacaoDto> arquivos,
                        Stream _,
                        CancellationToken _) =>
                    {
                        await foreach (var arquivo in arquivos)
                        {
                            _ = arquivo;
                        }
                    });

            var sut = CriarSut();

            // Act
            var excecao = await Record.ExceptionAsync(
                () => sut.ExecutarAsync(ids, streamSaida));

            // Assert
            Assert.NotNull(excecao);

            Assert.True(
                PossuiExcecao<InvalidOperationException>(excecao),
                $"Era esperada uma InvalidOperationException na cadeia. " +
                $"Exceção encontrada: {excecao}");

            servicoArmazenamento.Verify(
                x => x.ObterArquivoPorChaveAsync(
                    chave,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveTratarNomesDuplicadosAntesDaCompactacao()
        {
            // Arrange
            var declaracao1 = CriarDeclaracao(
                codigoDeclaracao: 55,
                chaveObjetoArmazenamento: "arquivo-1");

            var declaracao2 = CriarDeclaracao(
                codigoDeclaracao: 55,
                chaveObjetoArmazenamento: "arquivo-2");

            var ids = new List<long> { 1, 2 };

            var stream1 = new MemoryStream([1]);
            var stream2 = new MemoryStream([2]);

            var arquivosCompactados = new List<ArquivoCompactacaoDto>();

            repositorio
                .Setup(x => x.ObterDeclaracoesDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafDeclaracao>
                {
                    declaracao1,
                    declaracao2
                });

            servicoArmazenamento
                .Setup(x => x.ObterArquivoPorChaveAsync(
                    "arquivo-1",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Resultado<Stream>.DeSucesso(stream1));

            servicoArmazenamento
                .Setup(x => x.ObterArquivoPorChaveAsync(
                    "arquivo-2",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Resultado<Stream>.DeSucesso(stream2));

            await using var streamSaida = new MemoryStream();

            ConfigurarCompactadorConsumindoArquivos(
                streamSaida,
                arquivosCompactados);

            var sut = CriarSut();

            // Act
            await sut.ExecutarAsync(ids, streamSaida);

            // Assert
            Assert.Equal(2, arquivosCompactados.Count);

            Assert.Equal(
                2,
                arquivosCompactados
                    .Select(x => x.NomeArquivo)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count());

            Assert.Contains(
                arquivosCompactados,
                x => x.NomeArquivo.Contains(" (1).pdf"));

            servicoCompactacao.Verify(
                x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    streamSaida,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private CasoDeUsoDownloadLoteDeclaracoes CriarSut()
        {
            return new CasoDeUsoDownloadLoteDeclaracoes(
                repositorio.Object,
                servicoArmazenamento.Object,
                servicoCompactacao.Object,
                servicoRelatorio.Object);
        }

        private static CodafDeclaracao CriarDeclaracao(
            long codigoDeclaracao,
            string? chaveObjetoArmazenamento = null,
            string htmlContent = "<html>declaração</html>")
        {
            return new CodafDeclaracao(
                codafCursoNaoHomologadoInscricaoId: 100,
                tipoParticipacao: TipoParticipacaoCodaf.Cursista,
                referenciaId: 200,
                htmlContentSnapshot: htmlContent,
                metadadosJson: null)
            {
                CodigoDeclaracao = codigoDeclaracao,
                ChaveObjetoArmazenamento = chaveObjetoArmazenamento
            };
        }

        private void ConfigurarCompactadorConsumindoArquivos(
            Stream streamSaida,
            ICollection<ArquivoCompactacaoDto> arquivosRecebidos)
        {
            servicoCompactacao
                .Setup(x => x.CompactarAssincronamenteAsync(
                    It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(),
                    streamSaida,
                    It.IsAny<CancellationToken>()))
                .Returns(
                    async (
                        IAsyncEnumerable<ArquivoCompactacaoDto> arquivos,
                        Stream _,
                        CancellationToken _) =>
                    {
                        await foreach (var arquivo in arquivos)
                        {
                            arquivosRecebidos.Add(arquivo);
                        }
                    });
        }

        private static bool PossuiExcecao<TException>(Exception excecao)
            where TException : Exception
        {
            Exception? atual = excecao;

            while (atual != null)
            {
                if (atual is TException)
                    return true;

                atual = atual.InnerException;
            }

            return false;
        }
    }
}
