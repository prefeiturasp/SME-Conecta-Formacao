using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class GrupoControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterGrupoSistema> _mockUseCase;
        private readonly GrupoController _sut;

        public GrupoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockUseCase = mocker.GetMock<ICasoDeUsoObterGrupoSistema>();
            _sut = mocker.CreateInstance<GrupoController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterGrupos_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<GrupoDTO>();
            _mockUseCase.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterGrupos(_mockUseCase.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockUseCase.Verify(m => m.Executar(), Times.Once);
        }
    }
}
