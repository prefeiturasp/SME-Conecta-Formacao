using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarEmailEducacional;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarEmailEducacionalTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarEmailEducacional _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioAlterarEmailEducacionalTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioAlterarEmailEducacional>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveEnviarAlterarEmailEducacionalCommandERetornarTrue()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var email = _faker.Internet.Email();

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login, email);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoAlterarEmailEducacional_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var email = _faker.Internet.Email();

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(login, email);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var email = _faker.Internet.Email();
            var excecaoEsperada = new InvalidOperationException("Erro ao alterar e-mail educacional.");

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Executar(login, email);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro ao alterar e-mail educacional.");
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarEmailEducacionalCommand>(c => c.Email == email && c.Login == login), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
