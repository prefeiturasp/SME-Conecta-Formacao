using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriaPorId;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasPaginado;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CoordenadoriaControllerTestes
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IContextoAplicacao> _mockContexto;
        private readonly CoordenadoriaController _sut;

        public CoordenadoriaControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockMediator = mocker.GetMock<IMediator>();
            _mockContexto = mocker.GetMock<IContextoAplicacao>();
            _sut = mocker.CreateInstance<CoordenadoriaController>();
        }

        [Fact]
        public async Task DadoCadastroValido_QuandoCadastrar_EntaoRetornaCriado()
        {
            // Arrange
            var dto = new CoordenadoriaCadastroDto { Nome = "Teste", Sigla = "TST" };
            var retorno = Resultado<CoordenadoriaDto>.DeSucesso(new CoordenadoriaDto { Nome = "Teste" });
            _mockMediator.Setup(m => m.Send(It.IsAny<InserirCoordenadoriaCommand>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Cadastrar(dto, _mockMediator.Object) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.Created);
            _mockMediator.Verify(m => m.Send(It.IsAny<InserirCoordenadoriaCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoCadastroValido_QuandoAlterar_EntaoRetornaSucesso()
        {
            // Arrange
            var dto = new CoordenadoriaCadastroDto { Nome = "Teste", Sigla = "TST" };
            var retorno = Resultado.DeSucesso();
            _mockMediator.Setup(m => m.Send(It.IsAny<AlterarCoordenadoriaCommand>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Alterar(1, dto, _mockMediator.Object) as StatusCodeResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockMediator.Verify(m => m.Send(It.IsAny<AlterarCoordenadoriaCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterCoordenadoriasPaginado_EntaoRetornaPaginacao()
        {
            // Arrange
            var filtro = new CoordenadoriaFiltroDto { NumeroPagina = 1, NumeroRegistros = 10 };
            _mockContexto.Setup(m => m.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _mockContexto.Setup(m => m.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            var retorno = Resultado<PaginacaoResultadoDto<CoordenadoriaDto>>.DeSucesso(
                new PaginacaoResultadoDto<CoordenadoriaDto>(new List<CoordenadoriaDto>(), 0, 0));
            _mockMediator.Setup(m => m.Send(It.IsAny<ObterCoordenadoriasPaginadoQuery>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterCoordenadoriasPaginado(filtro, _mockMediator.Object, _mockContexto.Object) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockMediator.Verify(m => m.Send(It.IsAny<ObterCoordenadoriasPaginadoQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterPorId_EntaoRetornaDto()
        {
            // Arrange
            var id = 1L;
            var retorno = Resultado<CoordenadoriaDetalhadoDto>.DeSucesso(new CoordenadoriaDetalhadoDto { Nome = "Teste" });
            _mockMediator.Setup(m => m.Send(It.IsAny<ObterCoordenadoriaPorIdQuery>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterPorId(id, _mockMediator.Object) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockMediator.Verify(m => m.Send(It.IsAny<ObterCoordenadoriaPorIdQuery>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoExcluir_EntaoRetornaSucesso()
        {
            // Arrange
            var id = 1L;
            var retorno = Resultado.DeSucesso();
            _mockMediator.Setup(m => m.Send(It.IsAny<RemoverCoordenadoriaCommand>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.Excluir(id, _mockMediator.Object) as StatusCodeResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockMediator.Verify(m => m.Send(It.IsAny<RemoverCoordenadoriaCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoRequestValido_QuandoObterSelectCoordenadorias_EntaoRetornaLista()
        {
            // Arrange
            var retorno = new List<CoordenadoriaDto>();
            _mockMediator.Setup(m => m.Send(It.IsAny<ObterCoordenadoriasSelectQuery>(), default)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterSelectCoordenadorias(_mockMediator.Object);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(retorno);
            _mockMediator.Verify(m => m.Send(It.IsAny<ObterCoordenadoriasSelectQuery>(), default), Times.Once);
        }
    }
}
