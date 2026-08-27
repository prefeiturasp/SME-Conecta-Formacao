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
    public class AlterarSenhaServicoAcessosCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Faker _faker;
        private readonly AlterarSenhaServicoAcessosCommandHandler _handler;

        public AlterarSenhaServicoAcessosCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _handler = mocker.CreateInstance<AlterarSenhaServicoAcessosCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarMetodo_EntaoRetornaVerdadeiroSeAlteradaComSucesso()
        {
            // Arrange
            var comando = new AlterarSenhaServicoAcessosCommand(
                _faker.Internet.UserName(),
                _faker.Internet.Password(),
                _faker.Internet.Password());

            _servicoAcessosMock.Setup(s => s.AlterarSenha(comando.Login, comando.SenhaAtual, comando.NovaSenha))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _servicoAcessosMock.Verify(s => s.AlterarSenha(comando.Login, comando.SenhaAtual, comando.NovaSenha), Times.Once);
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarMetodo_EntaoRetornaFalsoSeAlteracaoFalhar()
        {
            // Arrange
            var comando = new AlterarSenhaServicoAcessosCommand(
                _faker.Internet.UserName(),
                _faker.Internet.Password(),
                _faker.Internet.Password());

            _servicoAcessosMock.Setup(s => s.AlterarSenha(comando.Login, comando.SenhaAtual, comando.NovaSenha))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _servicoAcessosMock.Verify(s => s.AlterarSenha(comando.Login, comando.SenhaAtual, comando.NovaSenha), Times.Once);
        }
    }
}
