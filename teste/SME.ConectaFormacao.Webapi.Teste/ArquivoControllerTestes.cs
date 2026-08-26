using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ArquivoControllerTestes
    {
        private readonly Mock<ICasoDeUsoArquivoCarregarTemporario> _mockCarregar;
        private readonly Mock<ICasoDeUsoArquivoExcluir> _mockExcluir;
        private readonly Mock<ICasoDeUsoArquivoBaixar> _mockBaixar;
        private readonly ArquivoController _sut;

        public ArquivoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockCarregar = mocker.GetMock<ICasoDeUsoArquivoCarregarTemporario>();
            _mockExcluir = mocker.GetMock<ICasoDeUsoArquivoExcluir>();
            _mockBaixar = mocker.GetMock<ICasoDeUsoArquivoBaixar>();
            _sut = mocker.CreateInstance<ArquivoController>();
        }

        [Fact]
        public async Task DadoArquivoValido_QuandoCarregar_EntaoRetornaArquivoArmazenado()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>().Object;
            var retorno = new ArquivoArmazenadoDTO(1, Guid.NewGuid(), "nome");
            _mockCarregar.Setup(m => m.Executar(arquivo)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Carregar(_mockCarregar.Object, arquivo) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockCarregar.Verify(m => m.Executar(arquivo), Times.Once);
        }

        [Fact]
        public async Task DadoCodigosValidos_QuandoExcluir_EntaoRetornaTrue()
        {
            // Arrange
            var codigos = new[] { Guid.NewGuid() };
            _mockExcluir.Setup(m => m.Executar(codigos)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Excluir(_mockExcluir.Object, codigos) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockExcluir.Verify(m => m.Executar(codigos), Times.Once);
        }

        [Fact]
        public async Task DadoCodigoValido_QuandoBaixar_EntaoRetornaFileStream()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var stream = new byte[] { 1 };
            var dto = new ArquivoBaixadoDTO { Arquivo = stream, Nome = "teste.txt", TipoConteudo = "text/plain" };
            _mockBaixar.Setup(m => m.Executar(codigo)).ReturnsAsync(dto);

            // Act
            var resultado = await _sut.Baixar(_mockBaixar.Object, codigo) as FileContentResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.FileContents.Should().BeEquivalentTo(dto.Arquivo);
            resultado.FileDownloadName.Should().Be(dto.Nome);
            resultado.ContentType.Should().Be(dto.TipoConteudo);
            _mockBaixar.Verify(m => m.Executar(codigo), Times.Once);
        }

        [Fact]
        public async Task DadoCodigoValidoParaNulo_QuandoBaixar_EntaoRetornaNoContent()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            _mockBaixar.Setup(m => m.Executar(codigo)).ReturnsAsync(default(ArquivoBaixadoDTO));

            // Act
            var resultado = await _sut.Baixar(_mockBaixar.Object, codigo) as NoContentResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockBaixar.Verify(m => m.Executar(codigo), Times.Once);
        }
    }
}
