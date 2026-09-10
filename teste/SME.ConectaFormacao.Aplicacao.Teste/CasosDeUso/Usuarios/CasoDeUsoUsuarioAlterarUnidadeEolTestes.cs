using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioAlterarUnidadeEolTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioAlterarUnidadeEol _sut;
        private readonly Faker _faker;

        public CasoDeUsoUsuarioAlterarUnidadeEolTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioAlterarUnidadeEol>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoAlteracaoRealizadaComSucesso_QuandoExecutar_EntaoEnviaCommandERetornaTrue()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var codigoEolUnidade = _faker.Random.Number(10000, 99999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarUnidadeEolUsuarioCommand>(c =>
                    c.Login == login && c.CodigoEolUnidade == codigoEolUnidade), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(login, codigoEolUnidade);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarUnidadeEolUsuarioCommand>(c =>
                    c.Login == login && c.CodigoEolUnidade == codigoEolUnidade),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoAlteracaoNaoRealizada_QuandoExecutar_EntaoEnviaCommandERetornaFalse()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var codigoEolUnidade = _faker.Random.Number(10000, 99999).ToString();

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarUnidadeEolUsuarioCommand>(c =>
                    c.Login == login && c.CodigoEolUnidade == codigoEolUnidade), default))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(login, codigoEolUnidade);

            // Assert
            resultado.Should().BeFalse();

            _mediatorMock.Verify(m => m.Send(
                It.Is<AlterarUnidadeEolUsuarioCommand>(c =>
                    c.Login == login && c.CodigoEolUnidade == codigoEolUnidade),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoPropagaExcecao()
        {
            // Arrange
            var login = _faker.Random.Number(1000000, 9999999).ToString();
            var codigoEolUnidade = _faker.Random.Number(10000, 99999).ToString();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AlterarUnidadeEolUsuarioCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(login, codigoEolUnidade);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
