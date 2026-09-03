using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class UsuarioRedeParceriaControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterSituacaoUsuarioRedeParceria> _mockObterSituacao;
        private readonly Mock<ICasoDeUsoObterUsuarioRedeParceriaPaginada> _mockObterPaginada;
        private readonly Mock<ICasoDeUsoObterUsuarioRedeParceriaPorId> _mockObterPorId;
        private readonly Mock<ICasoDeUsoInserirUsuarioRedeParceria> _mockInserir;
        private readonly Mock<ICasoDeUsoAlterarUsuarioRedeParceria> _mockAlterar;
        private readonly Mock<ICasoDeUsoRemoverUsuarioRedeParceria> _mockRemover;
        private readonly UsuarioRedeParceriaController _sut;

        public UsuarioRedeParceriaControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterSituacao = mocker.GetMock<ICasoDeUsoObterSituacaoUsuarioRedeParceria>();
            _mockObterPaginada = mocker.GetMock<ICasoDeUsoObterUsuarioRedeParceriaPaginada>();
            _mockObterPorId = mocker.GetMock<ICasoDeUsoObterUsuarioRedeParceriaPorId>();
            _mockInserir = mocker.GetMock<ICasoDeUsoInserirUsuarioRedeParceria>();
            _mockAlterar = mocker.GetMock<ICasoDeUsoAlterarUsuarioRedeParceria>();
            _mockRemover = mocker.GetMock<ICasoDeUsoRemoverUsuarioRedeParceria>();
            _sut = mocker.CreateInstance<UsuarioRedeParceriaController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterSituacao_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockObterSituacao.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterSituacao(_mockObterSituacao.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterSituacao.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterUsuarioRedeParceria_EntaoRetornaPaginacao()
        {
            // Arrange
            var filtro = new FiltroUsuarioRedeParceriaDTO();
            var retorno = new PaginacaoResultadoDto<UsuarioRedeParceriaPaginadoDTO>(new List<UsuarioRedeParceriaPaginadoDTO>(), 0, 0);
            _mockObterPaginada.Setup(m => m.Executar(filtro)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterUsuarioRedeParceria(_mockObterPaginada.Object, filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterPaginada.Verify(m => m.Executar(filtro), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterUsuarioRedeParceriaPorId_EntaoRetornaDto()
        {
            // Arrange
            var id = 1L;
            var retorno = new UsuarioRedeParceriaDTO();
            _mockObterPorId.Setup(m => m.Executar(id)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterUsuarioRedeParceriaPorId(_mockObterPorId.Object, id) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterPorId.Verify(m => m.Executar(id), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoInserirUsuarioRedeParceria_EntaoRetornaRetornoDto()
        {
            // Arrange
            var dto = new UsuarioRedeParceriaDTO();
            var retorno = new RetornoDTO();
            _mockInserir.Setup(m => m.Executar(dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.InserirUsuarioRedeParceria(_mockInserir.Object, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockInserir.Verify(m => m.Executar(dto), Times.Once);
        }

        [Fact]
        public async Task DadoIdEDtoValidos_QuandoAlterarUsuarioRedeParceria_EntaoRetornaRetornoDto()
        {
            // Arrange
            var id = 1L;
            var dto = new UsuarioRedeParceriaDTO();
            var retorno = new RetornoDTO();
            _mockAlterar.Setup(m => m.Executar(id, dto)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.AlterarUsuarioRedeParceria(_mockAlterar.Object, id, dto) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockAlterar.Verify(m => m.Executar(id, dto), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoRemoverUsuarioRedeParceria_EntaoRetornaRetornoDto()
        {
            // Arrange
            var id = 1L;
            var retorno = new RetornoDTO();
            _mockRemover.Setup(m => m.Executar(id)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.RemoverUsuarioRedeParceria(_mockRemover.Object, id) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockRemover.Verify(m => m.Executar(id), Times.Once);
        }
    }
}
