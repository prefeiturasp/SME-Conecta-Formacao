using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Notificacoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Notificacoes
{
    public class CasoDeUsoObterSituacaoNotificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterSituacaoNotificacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterSituacaoNotificacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterSituacaoNotificacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoConsultaSituacao_QuandoExecutar_EntaoDeveRetornarSituacoesNotificacao()
        {
            // Arrange
            var situacoesEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = _faker.Random.Long(1, 100), Descricao = _faker.Lorem.Word() },
                new() { Id = _faker.Random.Long(101, 200), Descricao = _faker.Lorem.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterNotificacaoSituacaoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(situacoesEsperadas);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(situacoesEsperadas);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoSituacaoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoConsultaSemRegistros_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var situacoesEsperadas = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterNotificacaoSituacaoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(situacoesEsperadas);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoSituacaoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterNotificacaoSituacaoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception(mensagemErro));

            // Act
            var act = async () => await _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterNotificacaoSituacaoQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
