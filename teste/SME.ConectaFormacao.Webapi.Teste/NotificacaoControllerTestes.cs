using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class NotificacaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterCategoriaNotificacao> _mockCategoria;
        private readonly Mock<ICasoDeUsoObterTipoNotificacao> _mockTipo;
        private readonly Mock<ICasoDeUsoObterSituacaoNotificacao> _mockSituacao;
        private readonly Mock<ICasoDeUsoObterTotalNotificacaoNaoLida> _mockTotalNaoLida;
        private readonly Mock<ICasoDeUsoObterNotificacao> _mockNotificacao;
        private readonly Mock<ICasoDeUsoObterNotificacaoPaginada> _mockNotificacaoPaginada;
        private readonly NotificacaoController _sut;

        public NotificacaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockCategoria = mocker.GetMock<ICasoDeUsoObterCategoriaNotificacao>();
            _mockTipo = mocker.GetMock<ICasoDeUsoObterTipoNotificacao>();
            _mockSituacao = mocker.GetMock<ICasoDeUsoObterSituacaoNotificacao>();
            _mockTotalNaoLida = mocker.GetMock<ICasoDeUsoObterTotalNotificacaoNaoLida>();
            _mockNotificacao = mocker.GetMock<ICasoDeUsoObterNotificacao>();
            _mockNotificacaoPaginada = mocker.GetMock<ICasoDeUsoObterNotificacaoPaginada>();
            _sut = mocker.CreateInstance<NotificacaoController>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterCategoriaNotificacao_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockCategoria.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterCategoriaNotificacao(_mockCategoria.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockCategoria.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterTipoNotificacao_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockTipo.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterTipoNotificacao(_mockTipo.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockTipo.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterSituacaoNotificacao_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<RetornoListagemDTO>();
            _mockSituacao.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterSituacaoNotificacao(_mockSituacao.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockSituacao.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterTotalNotificacaoNaoLida_EntaoRetornaLong()
        {
            // Arrange
            long retorno = 10;
            _mockTotalNaoLida.Setup(m => m.Executar()).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterTotalNotificacaoNaoLida(_mockTotalNaoLida.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(retorno);
            _mockTotalNaoLida.Verify(m => m.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterNotificacao_EntaoRetornaDto()
        {
            // Arrange
            long id = 1;
            var retorno = new NotificacaoDTO();
            _mockNotificacao.Setup(m => m.Executar(id)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterNotificacao(_mockNotificacao.Object, id) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockNotificacao.Verify(m => m.Executar(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterNotificacoes_EntaoRetornaPaginacao()
        {
            // Arrange
            var filtro = new NotificacaoFiltroDTO();
            var retorno = new PaginacaoResultadoDto<NotificacaoPaginadoDTO>(new List<NotificacaoPaginadoDTO>(), 0, 0);
            _mockNotificacaoPaginada.Setup(m => m.Executar(filtro)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterNotificacoes(_mockNotificacaoPaginada.Object, filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockNotificacaoPaginada.Verify(m => m.Executar(filtro), Times.Once);
        }
    }
}
