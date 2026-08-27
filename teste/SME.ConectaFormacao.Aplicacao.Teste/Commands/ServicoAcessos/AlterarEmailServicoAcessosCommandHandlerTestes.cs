using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ServicoAcessos
{
    public class AlterarEmailServicoAcessosCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Faker _faker;
        private readonly AlterarEmailServicoAcessosCommandHandler _handler;

        public AlterarEmailServicoAcessosCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _handler = mocker.CreateInstance<AlterarEmailServicoAcessosCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveRetornarTrue()
        {
            // Arrange
            var comando = new AlterarEmailServicoAcessosCommand(_faker.Internet.UserName(), _faker.Internet.Email());
            
            _servicoAcessosMock.Setup(s => s.AlterarEmail(comando.Login, comando.Email))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _servicoAcessosMock.Verify(s => s.AlterarEmail(comando.Login, comando.Email), Times.Once);
        }

        [Fact]
        public async Task DadoComandoComErro_QuandoExecutarHandle_DeveRetornarFalse()
        {
            // Arrange
            var comando = new AlterarEmailServicoAcessosCommand(_faker.Internet.UserName(), _faker.Internet.Email());
            
            _servicoAcessosMock.Setup(s => s.AlterarEmail(comando.Login, comando.Email))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _servicoAcessosMock.Verify(s => s.AlterarEmail(comando.Login, comando.Email), Times.Once);
        }
    }
}
