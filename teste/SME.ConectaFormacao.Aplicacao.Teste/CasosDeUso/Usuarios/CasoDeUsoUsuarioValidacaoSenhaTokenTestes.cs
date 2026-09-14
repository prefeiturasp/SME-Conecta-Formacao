using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Usuarios
{
    public class CasoDeUsoUsuarioValidacaoSenhaTokenTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioValidacaoSenhaToken _sut;

        public CasoDeUsoUsuarioValidacaoSenhaTokenTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoUsuarioValidacaoSenhaToken>();
        }

        [Fact]
        public async Task DadoTokenValido_QuandoExecutar_EntaoDeveRetornarTrue()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ValidarUsuarioTokenServicoAcessosQuery>(q => q.Token == token), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(token);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<ValidarUsuarioTokenServicoAcessosQuery>(q => q.Token == token), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTokenInvalido_QuandoExecutar_EntaoDeveRetornarFalse()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ValidarUsuarioTokenServicoAcessosQuery>(q => q.Token == token), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Executar(token);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.Is<ValidarUsuarioTokenServicoAcessosQuery>(q => q.Token == token), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarUsuarioTokenServicoAcessosQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha na validação do token"));

            // Act
            Func<Task> act = async () => await _sut.Executar(token);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Falha na validação do token");
            _mediatorMock.Verify(m => m.Send(It.IsAny<ValidarUsuarioTokenServicoAcessosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
