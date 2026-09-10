using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarTipoEmail;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarTipoEmailTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarTipoEmail _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioAlterarTipoEmailTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioAlterarTipoEmail>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoAmbosComandosComSucesso_QuandoExecutar_EntaoRetornaTrue()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var tipo = _faker.Random.Int(1, 3);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo), default))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login, tipo);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoPrimeiroComandoRetornaFalse_QuandoExecutar_EntaoRetornaFalseENaoChamaSegundoComando()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var tipo = _faker.Random.Int(1, 3);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo), default))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(login, tipo);

            // Assert
            resultado.Should().BeFalse();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(),
                default), Times.Never);
        }

        [Fact]
        public async Task DadoSegundoComandoRetornaFalse_QuandoExecutar_EntaoRetornaFalse()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var tipo = _faker.Random.Int(1, 3);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo), default))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login), default))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(login, tipo);

            // Assert
            resultado.Should().BeFalse();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarTipoEmailCommand>(c => c.Login == login && c.Tipo == tipo),
                default), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarEmailEduAoAlterarNomeTipoEmailCommand>(c => c.Login == login),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoPropagaExcecao()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var tipo = _faker.Random.Int(1, 3);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AlterarTipoEmailCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(login, tipo);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
