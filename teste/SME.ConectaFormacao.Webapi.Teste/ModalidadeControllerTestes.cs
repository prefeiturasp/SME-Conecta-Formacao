using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ModalidadeControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterModalidade> _mockObterModalidade;
        private readonly ModalidadeController _sut;

        public ModalidadeControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterModalidade = mocker.GetMock<ICasoDeUsoObterModalidade>();
            _sut = mocker.CreateInstance<ModalidadeController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterModalidade_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockObterModalidade.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterModalidade(_mockObterModalidade.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterModalidade.Verify(m => m.Executar(), Times.Once);
        }
    }
}
