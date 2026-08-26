using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class PalavraChaveControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterPalavraChave> _mockObterPalavraChave;
        private readonly PalavraChaveController _sut;

        public PalavraChaveControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterPalavraChave = mocker.GetMock<ICasoDeUsoObterPalavraChave>();
            _sut = mocker.CreateInstance<PalavraChaveController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterPalavraChave_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockObterPalavraChave.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterPalavraChave(_mockObterPalavraChave.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterPalavraChave.Verify(m => m.Executar(), Times.Once);
        }
    }
}
