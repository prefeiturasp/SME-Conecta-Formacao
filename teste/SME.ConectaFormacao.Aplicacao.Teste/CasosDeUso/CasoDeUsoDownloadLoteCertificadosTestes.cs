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
        private readonly CasoDeUsoDownloadLoteDeclaracoes _sut;
        private readonly Faker _faker;
        private readonly AutoMocker _mocker;

        private readonly Mock<IRepositorioCodafCertificado> _repositorioMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly Mock<IServicoCompactacao> _servicoCompactacaoMock;
        private readonly Mock<IServicoRelatorio> _servicoRelatorioMock;

        public CasoDeUsoDownloadLoteCertificadosTestes()
        {
            _mocker = new AutoMocker();
            _faker = new Faker("pt_BR");

            _repositorioMock = _mocker.GetMock<IRepositorioCodafCertificado>();
            _servicoArmazenamentoMock = _mocker.GetMock<IServicoArmazenamento>();
            _servicoCompactacaoMock = _mocker.GetMock<IServicoCompactacao>();
            _servicoRelatorioMock = _mocker.GetMock<IServicoRelatorio>();

            _sut = _mocker.CreateInstance<CasoDeUsoDownloadLoteDeclaracoes>();
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
        public async Task DadoListaDeIdsNula_QuandoExecutarAsync_EntaoDeveLancarArgumentException()
        {
            // Arrange
            List<long> idsNulos = null!;
            using var streamFake = new MemoryStream();

            // Act
            var acao = async () => await _sut.ExecutarAsync(idsNulos, streamFake);

            // Assert
            await acao.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*A lista de IDs não pode ser vazia*");
        }

        [Fact]
        public async Task DadoCertificadosNaoEncontrados_QuandoExecutarAsync_EntaoDeveLancarInvalidOperationException()
        {
            // Arrange
            var ids = new List<long> { 1, 2, 3 };
            using var streamFake = new MemoryStream();

            SetupRepositorioRetornando(ids, []);

            // Act
            var acao = async () => await _sut.ExecutarAsync(ids, streamFake);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Nenhum certificado encontrado para os IDs informados.");
        }

        [Fact]
        public async Task DadoCertificadoComChaveArmazenamentoValida_QuandoExecutarAsync_EntaoDeveObterStreamDoStorageECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();
            using var streamStorage = new MemoryStream(_faker.Random.Bytes(100));

            var certificado = CriarCertificadoFake("certificados/arquivo-123.pdf");

            SetupRepositorioRetornando(ids, [certificado]);
            SetupStorageRetornando(certificado.ChaveObjetoArmazenamento!, Resultado<Stream>.DeSucesso(streamStorage));

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            VerificarCompactacaoExecutada(Times.Once());
            _servicoRelatorioMock.Verify(s => s.ConveterHtmlCodafParaPdfAsync(It.IsAny<HtmlCodafDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoCertificadoSemChaveArmazenamento_QuandoExecutarAsync_EntaoDeveGerarPdfViaRelatorioECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();
            using var streamRelatorio = new MemoryStream(_faker.Random.Bytes(100));

            var certificado = CriarCertificadoFake(chaveArmazenamento: null);

            SetupRepositorioRetornando(ids, [certificado]);
            SetupRelatorioRetornando(Resultado<Stream>.DeSucesso(streamRelatorio));

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            _servicoArmazenamentoMock.Verify(s => s.ObterArquivoPorChaveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            VerificarCompactacaoExecutada(Times.Once());
        }

        [Fact]
        public async Task DadoCertificadoComChaveMasFalhaNoStorage_QuandoExecutarAsync_EntaoDeveFazerFallbackParaRelatorioECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();
            using var streamRelatorio = new MemoryStream(_faker.Random.Bytes(100));

            var certificado = CriarCertificadoFake("certificados/arquivo-falho.pdf");

            SetupRepositorioRetornando(ids, [certificado]);
            SetupStorageRetornando(certificado.ChaveObjetoArmazenamento!, Erro.NaoEncontrado());
            SetupRelatorioRetornando(Resultado<Stream>.DeSucesso(streamRelatorio));

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            _servicoArmazenamentoMock.Verify(s => s.ObterArquivoPorChaveAsync(certificado.ChaveObjetoArmazenamento!, It.IsAny<CancellationToken>()), Times.Once);
            _servicoRelatorioMock.Verify(s => s.ConveterHtmlCodafParaPdfAsync(It.IsAny<HtmlCodafDto>(), It.IsAny<CancellationToken>()), Times.Once);
            VerificarCompactacaoExecutada(Times.Once());
        }

        [Fact]
        public async Task DadoFalhaEmTodosOsServicos_QuandoExecutarAsync_EntaoDeveGerarTxtDeErroECompactar()
        {
            // Arrange
            var ids = new List<long> { 1 };
            using var streamSaida = new MemoryStream();

            var certificado = CriarCertificadoFake("chave-invalida");
            var erroRelatorio = Erro.NaoEncontrado(); // Assume que "Erro" preenche MensagensErro internamente

            SetupRepositorioRetornando(ids, [certificado]);
            SetupStorageRetornando(certificado.ChaveObjetoArmazenamento!, Erro.NaoEncontrado());
            SetupRelatorioRetornando(erroRelatorio);

            // Act
            await _sut.ExecutarAsync(ids, streamSaida);

            // Assert
            VerificarCompactacaoExecutada(Times.Once());
        }

        // --- MÉTODOS DE APOIO (KISS & DRY) ---

        private CodafCertificado CriarCertificadoFake(string? chaveArmazenamento = null)
        {
            return new CodafCertificado(
                codafId: _faker.Random.Long(1, 100),
                tipoCodaf: TipoCodaf.ListaPresenca,
                tipoParticipacao: TipoParticipacaoCodaf.Cursista,
                idReferencia: _faker.Random.Long(1, 100),
                htmlContentSnapshot: "<html>Fake</html>",
                metadadosJson: new { }
            )
            {
                CodigoCertificado = _faker.Random.Long(1000, 9999),
                ChaveObjetoArmazenamento = chaveArmazenamento
            };
        }

        private void SetupRepositorioRetornando(List<long> ids, IList<CodafCertificado> retorno)
        {
            _repositorioMock
                .Setup(r => r.ObterCertificadosDisponiveisPorListaDeIdAsync(ids))
                .ReturnsAsync(retorno);
        }

        private void SetupStorageRetornando(string chave, Resultado<Stream> retorno)
        {
            _servicoArmazenamentoMock
                .Setup(s => s.ObterArquivoPorChaveAsync(chave, It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);
        }

        private void SetupRelatorioRetornando(Resultado<Stream> retorno)
        {
            _servicoRelatorioMock
                .Setup(s => s.ConveterHtmlCodafParaPdfAsync(It.IsAny<HtmlCodafDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);
        }

        private void VerificarCompactacaoExecutada(Times times)
        {
            _servicoCompactacaoMock.Verify(s =>
                s.CompactarAssincronamenteAsync(It.IsAny<IAsyncEnumerable<ArquivoCompactacaoDto>>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
                times);
        }
    }
}