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
    public class CasoDeUsoRemoverPropostaTutorTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRemoverPropostaTutor _sut;
        private readonly Faker _faker;

        public CasoDeUsoRemoverPropostaTutorTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRemoverPropostaTutor>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoTutorIdValido_QuandoExecutar_EntaoDeveEnviarRemoverPropostaTutorCommandERetornarTrue()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(tutorId);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTutorIdValidoECommandRetornaFalse_QuandoExecutar_EntaoDeveRetornarFalse()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(tutorId);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1, 1000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(tutorId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.Is<RemoverPropostaTutorCommand>(c => c.TutorId == tutorId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
