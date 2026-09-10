using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoRemoverPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaExistente_QuandoExecutar_EntaoDeveEnviarRemoverPropostaCommandERetornarTrue()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoRemover_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var excecaoEsperada = new InvalidOperationException("Erro ao remover proposta.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao remover proposta.");
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
