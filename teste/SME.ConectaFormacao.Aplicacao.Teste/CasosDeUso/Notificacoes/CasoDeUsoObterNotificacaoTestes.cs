using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Notificacoes
{
    public class CasoDeUsoObterNotificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterNotificacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterNotificacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterNotificacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioLogadoENotificacaoExistente_QuandoExecutar_EntaoDeveRetornarNotificacaoDTO()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var notificacaoEsperada = new NotificacaoDTO
            {
                Id = id,
                Titulo = _faker.Lorem.Sentence(),
                Mensagem = _faker.Lorem.Paragraph()
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoQuery>(q => q.Id == id && q.Login == login),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(notificacaoEsperada);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(notificacaoEsperada);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterUsuarioLogadoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoQuery>(q => q.Id == id && q.Login == login),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoObterUsuarioLogado_QuandoExecutar_EntaoDevePropagarExcecaoENaoConsultarNotificacao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var mensagemErro = "Erro ao obter usuário logado";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoFalhaNaConsultaNotificacao_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var login = _faker.Internet.UserName();
            var usuarioLogado = new Usuario { Login = login };
            var mensagemErro = "Falha ao consultar notificação";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioLogado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoQuery>(q => q.Id == id && q.Login == login),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoQuery>(q => q.Id == id && q.Login == login),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
