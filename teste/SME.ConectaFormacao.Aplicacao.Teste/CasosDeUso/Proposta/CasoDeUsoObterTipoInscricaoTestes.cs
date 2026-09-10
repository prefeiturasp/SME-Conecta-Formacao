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
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTipoInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTipoInscricao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTipoInscricaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterTipoInscricao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRegistrosExistentes_QuandoExecutar_EntaoRetornaListaDeTiposInscricao()
        {
            // Arrange
            var retornoEsperado = new List<RetornoListagemDTO>
            {
                new() { Id = _faker.Random.Long(1, 100), Descricao = _faker.Random.Word() },
                new() { Id = _faker.Random.Long(101, 200), Descricao = _faker.Random.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeEquivalentTo(retornoEsperado);
            _mediatorMock.Verify(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoNenhumRegistroExistente_QuandoExecutar_EntaoRetornaColecaoVazia()
        {
            // Arrange
            var retornoEsperado = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().BeEmpty();
            _mediatorMock.Verify(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoPropagaExcecao()
        {
            // Arrange
            var excecaoEsperada = new InvalidOperationException("Erro ao consultar tipos de inscrição.");

            _mediatorMock
                .Setup(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao consultar tipos de inscrição.");
            _mediatorMock.Verify(m => m.Send(ObterTipoInscricaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
