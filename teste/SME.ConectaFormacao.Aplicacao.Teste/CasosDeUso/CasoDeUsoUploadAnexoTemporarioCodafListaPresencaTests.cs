using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoUploadAnexoTemporarioCodafListaPresencaTests
    {
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly CasoDeUsoUploadAnexoTemporarioCodafListaPresenca _casoDeUso;

        public CasoDeUsoUploadAnexoTemporarioCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoUploadAnexoTemporarioCodafListaPresenca>();
        }

        [Fact]
        public async Task DadoQueArquivoTenhaTamanhoZero_QuandoExecutarAsync_EntaoRetornaErroValidacao()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(0);
            var arquivoDto = arquivoMock.Object;

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(arquivoDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            _servicoArmazenamentoMock.Verify(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DadoQueArquivoExcedaTamanhoMaximo_QuandoExecutarAsync_EntaoRetornaErroValidacao()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(25 * 1024 * 1024); // 25 MB
            var arquivoDto = arquivoMock.Object;
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(arquivoDto);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            _servicoArmazenamentoMock.Verify(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DadoQueArquivoTenhaExtensaoInvalida_QuandoExecutarAsync_EntaoRetornaErroValidacao()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(1024); // 1 KB
            arquivoMock.Setup(a => a.FileName).Returns("documento.txt");
            arquivoMock.Setup(a => a.ContentType).Returns("text/plain");
            var arquivoDto = arquivoMock.Object;
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(arquivoDto);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            _servicoArmazenamentoMock.Verify(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DadoQueArquivoSejaValido_QuandoExecutarAsync_EntaoRetornaArquivoTemporarioDto()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(1024); // 1 KB
            arquivoMock.Setup(a => a.FileName).Returns("documento.pdf");
            arquivoMock.Setup(a => a.ContentType).Returns("application/pdf");
            arquivoMock.Setup(a => a.OpenReadStream()).Returns(new MemoryStream(Encoding.UTF8.GetBytes("Conteúdo do arquivo")));
            var arquivoDto = arquivoMock.Object;
            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(Guid.NewGuid());
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(arquivoDto);
            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.NomeArquivo.Should().Be("documento.pdf");
            resultado.Dados.ContentType.Should().Be("application/pdf");
            resultado.Dados.TamanhoBytes.Should().Be(1024);
            _servicoArmazenamentoMock.Verify(s => s.ArmazenarTemporariaGuid(It.IsAny<Stream>(), "application/pdf"), Times.Once);
        }
    }
}