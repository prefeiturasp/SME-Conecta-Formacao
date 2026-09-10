using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Notificacoes
{
    public class CasoDeUsoObterNotificacaoPaginadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterNotificacaoPaginada _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterNotificacaoPaginadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterNotificacaoPaginada>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltrosValidos_QuandoExecutar_EntaoDeveRetornarNotificacoesPaginadas()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var filtro = new NotificacaoFiltroDTO { Titulo = _faker.Lorem.Word() };
            var itens = new List<NotificacaoPaginadoDTO>
            {
                new() { Id = 1, Titulo = _faker.Lorem.Sentence() }
            };
            var resultadoEsperado = new PaginacaoResultadoDto<NotificacaoPaginadoDTO>(itens, 1, 10);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoPaginadaQuery>(q =>
                        q.Login == login &&
                        q.Filtro == filtro &&
                        q.NumeroPagina == _sut.NumeroPagina &&
                        q.NumeroRegistros == _sut.NumeroRegistros &&
                        q.QuantidadeRegistrosIgnorados == _sut.QuantidadeRegistrosIgnorados),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(resultadoEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoPaginadaQuery>(q =>
                    q.Login == login &&
                    q.Filtro == filtro &&
                    q.NumeroPagina == 1 &&
                    q.NumeroRegistros == 10 &&
                    q.QuantidadeRegistrosIgnorados == 0),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPaginacaoCustomizada_QuandoExecutar_EntaoDeveRepassarValoresCorretosParaQuery()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var filtro = new NotificacaoFiltroDTO();

            _sut.NumeroPagina = 3;
            _sut.NumeroRegistros = 15;
            _sut.QuantidadeRegistrosIgnorados = 30;

            var resultadoEsperado = new PaginacaoResultadoDto<NotificacaoPaginadoDTO>(
                new List<NotificacaoPaginadoDTO>(), 50, 15);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoPaginadaQuery>(q =>
                        q.Login == login &&
                        q.Filtro == filtro &&
                        q.NumeroPagina == 3 &&
                        q.NumeroRegistros == 15 &&
                        q.QuantidadeRegistrosIgnorados == 30),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(resultadoEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoPaginadaQuery>(q =>
                    q.Login == login &&
                    q.Filtro == filtro &&
                    q.NumeroPagina == 3 &&
                    q.NumeroRegistros == 15 &&
                    q.QuantidadeRegistrosIgnorados == 30),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoObterUsuarioLogado_QuandoExecutar_EntaoDevePropagarExcecaoENaoConsultarNotificacaoPaginada()
        {
            // Arrange
            var filtro = new NotificacaoFiltroDTO();
            var mensagemErro = "Erro ao obter usuário logado";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(filtro);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoPaginadaQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoFalhaNaConsultaPaginada_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var filtro = new NotificacaoFiltroDTO();
            var mensagemErro = "Falha na consulta paginada";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterNotificacaoPaginadaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(filtro);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoPaginadaQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
