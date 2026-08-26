using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using SME.ConectaFormacao.Aplicacao.Interfaces.Dre;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class DreControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterListaDre> _mockObterLista;
        private readonly Mock<ICasoDeUsoObterDreListaUsuarioLogado> _mockObterLogado;
        private readonly DreController _sut;

        public DreControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterLista = mocker.GetMock<ICasoDeUsoObterListaDre>();
            _mockObterLogado = mocker.GetMock<ICasoDeUsoObterDreListaUsuarioLogado>();
            _sut = mocker.CreateInstance<DreController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterListaDre_EntaoRetornaLista()
        {
            // Arrange
            var exibirTodos = true;
            var retorno = new List<DreDTO>();
            _mockObterLista.Setup(m => m.Executar(exibirTodos)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterListaDre(_mockObterLista.Object, exibirTodos) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterLista.Verify(m => m.Executar(exibirTodos), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterDreListaUsuarioLogado_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<DreDTO>();
            _mockObterLogado.Setup(m => m.ExecutarAsync()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterDreListaUsuarioLogado(_mockObterLogado.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterLogado.Verify(m => m.ExecutarAsync(), Times.Once);
        }
    }
}
