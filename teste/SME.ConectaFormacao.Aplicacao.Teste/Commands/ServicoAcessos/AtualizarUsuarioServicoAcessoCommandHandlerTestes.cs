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
    public class AtualizarUsuarioServicoAcessoCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Faker _faker;
        private readonly AtualizarUsuarioServicoAcessoCommandHandler _handler;

        public AtualizarUsuarioServicoAcessoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _handler = mocker.CreateInstance<AtualizarUsuarioServicoAcessoCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_EntaoDeveRetornarTrue()
        {
            // Arrange
            var comando = new AtualizarUsuarioServicoAcessoCommand(_faker.Internet.UserName(), _faker.Person.FullName, _faker.Internet.Email(), _faker.Internet.Password())
            {
                NomeSocial = _faker.Person.FirstName
            };
            
            _servicoAcessosMock.Setup(s => s.AtualizarUsuarioCoreSSO(comando.Login, comando.Nome, comando.Email, comando.Senha, comando.NomeSocial))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _servicoAcessosMock.Verify(s => s.AtualizarUsuarioCoreSSO(comando.Login, comando.Nome, comando.Email, comando.Senha, comando.NomeSocial), Times.Once);
        }

        [Fact]
        public async Task DadoComandoComErro_QuandoExecutarHandle_EntaoDeveRetornarFalse()
        {
            // Arrange
            var comando = new AtualizarUsuarioServicoAcessoCommand(_faker.Internet.UserName(), _faker.Person.FullName, _faker.Internet.Email(), _faker.Internet.Password())
            {
                NomeSocial = _faker.Person.FirstName
            };
            
            _servicoAcessosMock.Setup(s => s.AtualizarUsuarioCoreSSO(comando.Login, comando.Nome, comando.Email, comando.Senha, comando.NomeSocial))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _servicoAcessosMock.Verify(s => s.AtualizarUsuarioCoreSSO(comando.Login, comando.Nome, comando.Email, comando.Senha, comando.NomeSocial), Times.Once);
        }
    }
}
