using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class FuncionarioControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterUsuariosAdminDf> _mockObterAdminDf;
        private readonly Mock<ICasoDeUsoObterParecerista> _mockObterParecerista;
        private readonly FuncionarioController _sut;

        public FuncionarioControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterAdminDf = mocker.GetMock<ICasoDeUsoObterUsuariosAdminDf>();
            _mockObterParecerista = mocker.GetMock<ICasoDeUsoObterParecerista>();
            _sut = mocker.CreateInstance<FuncionarioController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterUsuariosAdminDf_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoUsuarioLoginNomeDTO>();
            _mockObterAdminDf.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterUsuariosAdminDf(_mockObterAdminDf.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterAdminDf.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterParecerista_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoUsuarioLoginNomeDTO>();
            _mockObterParecerista.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterParecerista(_mockObterParecerista.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterParecerista.Verify(m => m.Executar(), Times.Once);
        }
    }
}
