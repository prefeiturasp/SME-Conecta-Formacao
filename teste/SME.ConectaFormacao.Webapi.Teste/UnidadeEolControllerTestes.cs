using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class UnidadeEolControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterUnidadePorCodigoEol> _mockUseCase;
        private readonly UnidadeEolController _sut;

        public UnidadeEolControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockUseCase = mocker.GetMock<ICasoDeUsoObterUnidadePorCodigoEol>();
            _sut = mocker.CreateInstance<UnidadeEolController>();
        }

        [Fact]
        public async Task DadoCodigoEolValido_QuandoBuscarUnidadePorCodigoEol_EntaoRetornaUnidade()
        {
            // Arrange
            var codigoEol = "123456";
            var retorno = new UnidadeEol();
            _mockUseCase.Setup(m => m.Executar(codigoEol)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.BuscarUnidadePorCodigoEol(codigoEol, _mockUseCase.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockUseCase.Verify(m => m.Executar(codigoEol), Times.Once);
        }
    }
}
