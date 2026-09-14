using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Notificacoes
{
    public class CasoDeUsoObterTotalNotificacaoNaoLidaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTotalNotificacaoNaoLida _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTotalNotificacaoNaoLidaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterTotalNotificacaoNaoLida>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogado_QuandoExecutar_EntaoDeveRetornarTotalNotificacoesNaoLidas()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var totalEsperado = _faker.Random.Long(1, 100);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>(q => q.Login == login),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().Be(totalEsperado);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>(q => q.Login == login),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoObterUsuarioLogado_QuandoExecutar_EntaoDevePropagarExcecaoENaoConsultarTotal()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoFalhaAoObterTotalNotificacoes_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>(q => q.Login == login),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterTotalNotificacaoNaoLidaPorUsuarioQuery>(q => q.Login == login),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
