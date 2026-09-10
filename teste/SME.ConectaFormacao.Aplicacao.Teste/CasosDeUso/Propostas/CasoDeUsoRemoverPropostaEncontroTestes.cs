using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoRemoverPropostaEncontroTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverPropostaEncontro _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverPropostaEncontroTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverPropostaEncontro>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaEncontroExistente_QuandoExecutar_EntaoDeveEnviarRemoverPropostaEncontroCommandERetornarTrue()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoRemover_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var excecaoEsperada = new InvalidOperationException("Erro ao remover encontro da proposta.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao remover encontro da proposta.");
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaEncontroCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
