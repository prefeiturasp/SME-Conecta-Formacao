using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
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
    public class CasoDeUsoObterTipoNotificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTipoNotificacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTipoNotificacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterTipoNotificacao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoTiposNotificacaoExistentes_QuandoExecutar_EntaoDeveEnviarQueryERetornarColecaoEsperada()
        {
            // Arrange
            var tiposEsperados = new List<RetornoListagemDTO>
            {
                new() { Id = _faker.Random.Long(1, 100), Descricao = _faker.Lorem.Word() },
                new() { Id = _faker.Random.Long(101, 200), Descricao = _faker.Lorem.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(tiposEsperados);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoNenhumTipoNotificacaoCadastrado_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var colecaoVazia = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(colecaoVazia);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();
            var excecaoEsperada = new InvalidOperationException(mensagemErro);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterNotificacaoTipoQuery>(q => q == ObterNotificacaoTipoQuery.Instancia()),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
