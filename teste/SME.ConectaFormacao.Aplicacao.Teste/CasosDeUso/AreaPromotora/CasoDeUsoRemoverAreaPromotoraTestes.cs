using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoRemoverAreaPromotoraTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverAreaPromotora _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverAreaPromotoraTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverAreaPromotora>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoIdValido_QuandoExecutar_EntaoDeveEnviarCommandERetornarTrue()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverAreaPromotoraCommand>(c => c.Id == id), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverAreaPromotoraCommand>(c => c.Id == id), default), Times.Once);
        }

        [Fact]
        public async Task DadoRemocaoNaoRealizada_QuandoExecutar_EntaoDeveRetornarFalse()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverAreaPromotoraCommand>(c => c.Id == id), default))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverAreaPromotoraCommand>(c => c.Id == id), default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RemoverAreaPromotoraCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
