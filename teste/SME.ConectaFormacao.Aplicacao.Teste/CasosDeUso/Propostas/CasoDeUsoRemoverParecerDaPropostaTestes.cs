using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoRemoverParecerDaPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverParecerDaProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverParecerDaPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverParecerDaProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoParecerExistente_QuandoExecutar_EntaoDeveEnviarExcluirParecerCommandERetornarTrue()
        {
            // Arrange
            var parecerId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(parecerId);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoExcluir_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var parecerId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(parecerId);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var parecerId = _faker.Random.Long(1, 1000);
            var excecaoEsperada = new InvalidOperationException("Erro ao remover parecer.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(parecerId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao remover parecer.");
            _mediatorMock.Verify(m => m.Send(It.Is<ExcluirParecerCommand>(c => c.ParecerId == parecerId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
