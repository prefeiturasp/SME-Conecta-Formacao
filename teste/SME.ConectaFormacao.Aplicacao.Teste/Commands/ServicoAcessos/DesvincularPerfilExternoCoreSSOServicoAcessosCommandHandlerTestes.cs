using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ServicoAcessos
{
    public class DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandler _sut;
        private readonly Faker _faker;

        public DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _sut = mocker.CreateInstance<DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public void DadoServicoAcessosNulo_QuandoConstruir_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IServicoAcessos servicoAcessosNulo = null!;

            // Act
            var act = () => new DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandler(servicoAcessosNulo);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("servicoAcessos");
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_EntaoDeveChamarServicoERetornarTrue()
        {
            // Arrange
            var comando = new DesvincularPerfilExternoCoreSSOServicoAcessosCommand(_faker.Internet.UserName(), Guid.NewGuid());

            _servicoAcessosMock
                .Setup(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _servicoAcessosMock.Verify(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaAoDesvincular_QuandoExecutarHandle_EntaoRetornaFalse()
        {
            // Arrange
            var comando = new DesvincularPerfilExternoCoreSSOServicoAcessosCommand(_faker.Internet.UserName(), Guid.NewGuid());

            _servicoAcessosMock
                .Setup(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _servicoAcessosMock.Verify(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoServico_QuandoExecutarHandle_EntaoDevePropagarExcecao()
        {
            // Arrange
            var comando = new DesvincularPerfilExternoCoreSSOServicoAcessosCommand(_faker.Internet.UserName(), Guid.NewGuid());
            var excecaoEsperada = new InvalidOperationException("Erro no serviço de acessos.");

            _servicoAcessosMock
                .Setup(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var act = () => _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Erro no serviço de acessos.");
            _servicoAcessosMock.Verify(s => s.DesvincularPerfilExternoCoreSSO(comando.Login, comando.PerfilId), Times.Once);
        }
    }
}
