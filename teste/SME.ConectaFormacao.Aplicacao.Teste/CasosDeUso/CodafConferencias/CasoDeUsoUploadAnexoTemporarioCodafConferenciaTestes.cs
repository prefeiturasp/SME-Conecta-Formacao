using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoUploadAnexoTemporarioCodafConferenciaTestes
    {
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly CasoDeUsoUploadAnexoTemporarioCodafConferencia _sut;

        public CasoDeUsoUploadAnexoTemporarioCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _sut = mocker.CreateInstance<CasoDeUsoUploadAnexoTemporarioCodafConferencia>();
        }

        [Fact]
        public async Task DadoArquivoNulo_QuandoExecutar_EntaoDeveRetornarErroValidacao()
        {
            // Act
            var resultado = await _sut.ExecutarAsync(null!);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            resultado.MensagensErro.Should().Contain("Nenhum arquivo foi enviado.");
        }

        [Fact]
        public async Task DadoArquivoNaoPdf_QuandoExecutar_EntaoDeveRetornarErroValidacao()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(1024);
            arquivoMock.Setup(a => a.FileName).Returns("imagem.png");
            arquivoMock.Setup(a => a.ContentType).Returns("image/png");

            // Act
            var resultado = await _sut.ExecutarAsync(arquivoMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoArquivoPdfValido_QuandoExecutar_EntaoDeveRetornarAnexoTemporario()
        {
            // Arrange
            var arquivoId = Guid.NewGuid();
            var stream = new MemoryStream([1, 2, 3]);
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(3);
            arquivoMock.Setup(a => a.FileName).Returns("documento.pdf");
            arquivoMock.Setup(a => a.ContentType).Returns("application/pdf");
            arquivoMock.Setup(a => a.OpenReadStream()).Returns(stream);

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), "application/pdf"))
                .ReturnsAsync(arquivoId);
            _servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(arquivoId.ToString(), true))
                .ReturnsAsync("https://storage/temp/documento.pdf");

            // Act
            var resultado = await _sut.ExecutarAsync(arquivoMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados!.ArquivoCodigo.Should().Be(arquivoId);
            resultado.Dados.NomeArquivo.Should().Be("documento.pdf");
            resultado.Dados.Extensao.Should().Be(".pdf");
        }
    }
}
