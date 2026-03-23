using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;
using SME.ConectaFormacao.Infra.Servicos.Compactacao.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoDownloadLoteCertificadosTestes
    {
        private readonly CasoDeUsoDownloadLoteCertificados _sut;
        private readonly Faker _faker;

        private readonly Mock<IRepositorioCodafCertificado> _repositorioMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly Mock<IServicoCompactacao> _servicoCompactacaoMock;
        private readonly Mock<IServicoRelatorio> _servicoRelatorioMock;

        public CasoDeUsoDownloadLoteCertificadosTestes()
        {
            var _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            _repositorioMock = _mocker.GetMock<IRepositorioCodafCertificado>();
            _servicoArmazenamentoMock = _mocker.GetMock<IServicoArmazenamento>();
            _servicoCompactacaoMock = _mocker.GetMock<IServicoCompactacao>();
            _servicoRelatorioMock = _mocker.GetMock<IServicoRelatorio>();

            _sut = _mocker.CreateInstance<CasoDeUsoDownloadLoteCertificados>();
        }

        [Fact]
        public async Task DadoListaDeIdsVazia_QuandoExecutarAsync_EntaoDeveLancarArgumentException()
        {
            // Arrange
            var idsVazios = new List<long>();
            using var streamFake = new MemoryStream();

            // Act
            var acao = async () => await _sut.ExecutarAsync(idsVazios, streamFake);

            // Assert
            await acao.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*A lista de IDs não pode ser vazia*");
        }

        [Fact]
        public async Task DadoNenhumCertificadoEncontradoNoBanco_QuandoExecutarAsync_EntaoDeveLancarInvalidOperationException()
        {
            // Arrange
            var ids = new List<long> { 1, 2, 3 };
            using var streamFake = new MemoryStream();

            _repositorioMock
                .Setup(r => r.ObterCertificadosDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafCertificado>());

            // Act
            Func<Task> acao = async () => await _sut.ExecutarAsync(ids, streamFake);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Nenhum certificado encontrado para os IDs informados.");
        }

        [Fact]
        public async Task DadoCertificadoValidoComChaveArmazenamento_QuandoExecutarAsync_EntaoDeveObterStreamDoMinioECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();
            using var streamMinio = new MemoryStream(_faker.Random.Bytes(100));

            var certificado = CriarCertificadoFake();
            certificado.ChaveObjetoArmazenamento = "certificados/arquivo-123.pdf";

            _repositorioMock
                .Setup(r => r.ObterCertificadosDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync([certificado]);

            var resultadoSucesso = Resultado<Stream>.DeSucesso(streamMinio);

            _servicoArmazenamentoMock
                .Setup(s => s.ObterArquivoPorChaveAsync(certificado.ChaveObjetoArmazenamento, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoSucesso);

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            _servicoCompactacaoMock.Verify(s =>
                s.CompactarAssincronamenteAsync(It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(), streamSaida, It.IsAny<CancellationToken>()),
                Times.Once);

            _servicoRelatorioMock.Verify(s =>
                s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoCertificadoSemChaveArmazenamento_QuandoExecutarAsync_EntaoDeveGerarPdfViaRelatorioECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();
            using var streamRelatorio = new MemoryStream(_faker.Random.Bytes(100));

            var certificado = CriarCertificadoFake();
            certificado.ChaveObjetoArmazenamento = null;

            _repositorioMock
                .Setup(r => r.ObterCertificadosDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafCertificado> { certificado });

            var resultadoSucesso = Resultado<Stream>.DeSucesso(streamRelatorio);

            _servicoRelatorioMock
                .Setup(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoSucesso);

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            _servicoArmazenamentoMock.Verify(s =>
                s.ObterArquivoPorChaveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _servicoCompactacaoMock.Verify(s =>
                s.CompactarAssincronamenteAsync(It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(), streamSaida, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoFalhaEmTodosOsServicos_QuandoExecutarAsync_EntaoDeveGerarTxtComMensagemDeErroECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();

            var certificado = CriarCertificadoFake();
            certificado.ChaveObjetoArmazenamento = "chave-invalida";

            _repositorioMock
                .Setup(r => r.ObterCertificadosDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(new List<CodafCertificado> { certificado });

            var falhaMinio = Erro.NaoEncontrado();
            _servicoArmazenamentoMock
                .Setup(s => s.ObterArquivoPorChaveAsync(certificado.ChaveObjetoArmazenamento, It.IsAny<CancellationToken>()))
                .ReturnsAsync(falhaMinio);

            var falhaRelatorio = Erro.NaoEncontrado();
            _servicoRelatorioMock
                .Setup(s => s.ConveterHtmlCertificadoCodafParaPdfAsync(It.IsAny<HtmlCertificadoCodafDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(falhaRelatorio);

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            _servicoCompactacaoMock.Verify(s =>
                s.CompactarAssincronamenteAsync(It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(), streamSaida, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // --- Helper Methods ---
        private CodafCertificado CriarCertificadoFake()
        {
            return new CodafCertificado(
                codafListaPresencaId: _faker.Random.Long(1, 100),
                tipoParticipacao: TipoParticipacaoCodaf.Cursista,
                idReferencia: _faker.Random.Long(1, 100),
                htmlContentSnapshot: "<html>Fake</html>",
                metadadosJson: new { }
            )
            {
                CodigoCertificado = _faker.Random.Long(1000, 9999)
            };
        }
    }
}
