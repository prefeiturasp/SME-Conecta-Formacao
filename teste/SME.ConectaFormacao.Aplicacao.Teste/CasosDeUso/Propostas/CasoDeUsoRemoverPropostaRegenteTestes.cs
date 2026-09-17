using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoRemoverPropostaRegenteTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverPropostaRegente _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverPropostaRegenteTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverPropostaRegente>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaRegenteExistente_QuandoExecutar_EntaoDeveEnviarRemoverPropostaRegenteCommandERetornarTrue()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(regenteId);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoRemover_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(regenteId);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);
            var excecaoEsperada = new InvalidOperationException("Erro ao remover regente da proposta.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(regenteId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao remover regente da proposta.");
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaRegenteCommand>(c => c.RegenteId == regenteId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
