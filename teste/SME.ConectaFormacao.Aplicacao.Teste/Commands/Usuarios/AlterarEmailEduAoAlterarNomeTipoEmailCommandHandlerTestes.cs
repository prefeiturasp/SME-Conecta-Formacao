using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class AlterarEmailEduAoAlterarNomeTipoEmailCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly AlterarEmailEduAoAlterarNomeTipoEmailCommandHandler _handler;

        public AlterarEmailEduAoAlterarNomeTipoEmailCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _handler = mocker.CreateInstance<AlterarEmailEduAoAlterarNomeTipoEmailCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveAlterarEmailESalvar()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var comando = new AlterarEmailEduAoAlterarNomeTipoEmailCommand(login);
            var usuario = new Usuario { Login = login };
            var novoEmail = _faker.Internet.Email();

            _mediatorMock.Setup(m => m.Send(It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == login), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mediatorMock.Setup(m => m.Send(It.Is<GerarEmailEducacionalCommand>(c => c.Usuario == usuario), It.IsAny<CancellationToken>()))
                .ReturnsAsync(novoEmail);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            usuario.EmailEducacional.Should().Be(novoEmail);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == login), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<GerarEmailEducacionalCommand>(c => c.Usuario == usuario), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarUsuarioCommand>(c => c.Usuario == usuario && c.AlterouNome == true), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
