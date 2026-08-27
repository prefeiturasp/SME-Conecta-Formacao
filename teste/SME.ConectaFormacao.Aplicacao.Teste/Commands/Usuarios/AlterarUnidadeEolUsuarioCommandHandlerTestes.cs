using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class AlterarUnidadeEolUsuarioCommandHandlerTestes
    {
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly AlterarUnidadeEolUsuarioCommandHandler _handler;

        public AlterarUnidadeEolUsuarioCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _handler = mocker.CreateInstance<AlterarUnidadeEolUsuarioCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUsuarioInexistente_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarUnidadeEolUsuarioCommand(_faker.Random.String2(10), _faker.Random.String2(10));
            
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin(comando.Login))
                .ReturnsAsync((Usuario)null);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveAtualizarUsuarioERetornarTrue()
        {
            // Arrange
            var comando = new AlterarUnidadeEolUsuarioCommand(_faker.Random.String2(10), _faker.Random.String2(10));
            var usuario = new Usuario
            {
                Login = comando.Login
            };
            
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin(comando.Login))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            usuario.CodigoEolUnidade.Should().Be(comando.CodigoEolUnidade);
            _repositorioUsuarioMock.Verify(r => r.Atualizar(usuario), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
