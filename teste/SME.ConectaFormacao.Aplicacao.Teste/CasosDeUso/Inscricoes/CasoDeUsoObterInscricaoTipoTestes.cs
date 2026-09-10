using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterInscricaoTipoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterInscricaoTipo _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterInscricaoTipoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterInscricaoTipo>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoChamadaParaObterTipos_QuandoExecutar_EntaoDeveEnviarQueryERetornarListaDeTipos()
        {
            // Arrange
            var tiposEsperados = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = _faker.Random.Word() },
                new() { Id = 2, Descricao = _faker.Random.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposEsperados);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeEquivalentTo(tiposEsperados);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoQuerySemResultados_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var esperadoVazio = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(esperadoVazio);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeEmpty();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterInscricaoTipoListaQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
