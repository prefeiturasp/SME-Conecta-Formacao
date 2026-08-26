using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CargoFuncaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterCargoFuncao> _mockObterCargo;
        private readonly CargoFuncaoController _sut;

        public CargoFuncaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterCargo = mocker.GetMock<ICasoDeUsoObterCargoFuncao>();
            _sut = mocker.CreateInstance<CargoFuncaoController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterCargoFuncao_EntaoRetornaLista()
        {
            // Arrange
            var tipo = (CargoFuncaoTipo)1;
            var exibirOpcaoOutros = true;
            var retornoDesejado = new List<CargoFuncaoDto>();

            _mockObterCargo.Setup(m => m.Executar(tipo, exibirOpcaoOutros)).ReturnsAsync(retornoDesejado);

            // Act
            var resultado = await _sut.ObterCargoFuncao(_mockObterCargo.Object, tipo, exibirOpcaoOutros) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retornoDesejado);
            _mockObterCargo.Verify(m => m.Executar(tipo, exibirOpcaoOutros), Times.Once);
        }
    }
}
