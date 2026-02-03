using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCertificadoCodafParaDownloadTests
    {
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorioCodafCertificado;
        private readonly Mock<IServicoArmazenamento> _mockServicoArmazenamento;
        private readonly CasoDeUsoObterCertificadoCodafParaDownload _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterCertificadoCodafParaDownloadTests()
        {
            var mocker = new AutoMocker();
            _mockRepositorioCodafCertificado = mocker.GetMock<IRepositorioCodafCertificado>();
            _mockServicoArmazenamento = mocker.GetMock<IServicoArmazenamento>();
            _sut = mocker.CreateInstance<CasoDeUsoObterCertificadoCodafParaDownload>();
            _faker = new();
        }

        [Fact]
        public async Task DadoCertificadoNaoEncontrado_QuandoExecutarAsync_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            long certificadoCodafId = _faker.Random.Long(1, 1000);

            // Act
            var resultado = await _sut.ExecutarAsync(certificadoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoCertificadoSemChaveDeArmazenamento_QuandoExecutarAsync_EntaoDeveRetornarErroValidacao()
        {
            // Arrange
            long certificadoCodafId = _faker.Random.Long(1, 1000);
            var certificadoCodaf = new DadosCertificadoUsuarioParaDownloadDto
            {
                CodigoCertificado = _faker.Random.Long(1, 1000),
                Id = certificadoCodafId,
                NomeCompleto = _faker.Person.FullName,
                NomeFormacao = _faker.Commerce.ProductName()
            };
            _mockRepositorioCodafCertificado
                .Setup(r => r.ObterCertificadoDisponivelDoUsuarioAsync(certificadoCodafId))
                .ReturnsAsync(certificadoCodaf);

            // Act
            var resultado = await _sut.ExecutarAsync(certificadoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoServicoArmazenamentoNaoRetornaUrl_QuandoExecutarAsync_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            long certificadoCodafId = _faker.Random.Long(1, 1000);
            var certificadoCodaf = new DadosCertificadoUsuarioParaDownloadDto
            {
                CodigoCertificado = _faker.Random.Long(1, 1000),
                Id = certificadoCodafId,
                NomeCompleto = _faker.Person.FullName,
                NomeFormacao = _faker.Commerce.ProductName(),
                ChaveObjetoArmazenamento = _faker.Random.AlphaNumeric(20)
            };
            _mockRepositorioCodafCertificado
                .Setup(r => r.ObterCertificadoDisponivelDoUsuarioAsync(certificadoCodafId))
                .ReturnsAsync(certificadoCodaf);

            // Act
            var resultado = await _sut.ExecutarAsync(certificadoCodafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoCertificadoValido_QuandoExecutarAsync_EntaoDeveRetornarUrlParaDownload()
        {
            // Arrange
            long certificadoCodafId = _faker.Random.Long(1, 1000);
            var certificadoCodaf = new DadosCertificadoUsuarioParaDownloadDto
            {
                CodigoCertificado = _faker.Random.Long(1, 1000),
                Id = certificadoCodafId,
                NomeCompleto = _faker.Person.FullName,
                NomeFormacao = _faker.Commerce.ProductName(),
                ChaveObjetoArmazenamento = _faker.Random.AlphaNumeric(20)
            };
            var urlDownloadEsperada = "https://armazenamento.exemplo.com/certificado.pdf";
            _mockRepositorioCodafCertificado
                .Setup(r => r.ObterCertificadoDisponivelDoUsuarioAsync(certificadoCodafId))
                .ReturnsAsync(certificadoCodaf);
            _mockServicoArmazenamento
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(certificadoCodaf.ChaveObjetoArmazenamento))
                .ReturnsAsync(urlDownloadEsperada);
            // Act
            var resultado = await _sut.ExecutarAsync(certificadoCodafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.UrlDownload.Should().Be(urlDownloadEsperada);
            resultado.Dados.CodigoCertificado.Should().Be(certificadoCodaf.CodigoCertificado);
            resultado.Dados.Id.Should().Be(certificadoCodaf.Id);
            resultado.Dados.NomeCompleto.Should().Be(certificadoCodaf.NomeCompleto);
            resultado.Dados.NomeFormacao.Should().Be(certificadoCodaf.NomeFormacao);
        }
    }
}