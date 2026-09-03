using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ServicoAcessos
{
    public class AlterarNomeServicoAcessosCommandHandlerTestes
    {
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly AlterarNomeServicoAcessosCommandHandler _handler;

        public AlterarNomeServicoAcessosCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _handler = mocker.CreateInstance<AlterarNomeServicoAcessosCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveRemoverCacheEAlterarNomeRetornandoTrue()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var nome = _faker.Person.FullName;
            var comando = new AlterarNomeServicoAcessosCommand(login, nome);

            _servicoAcessosMock.Setup(s => s.AlterarNome(login, nome))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(
                It.Is<RemoverCacheCommand>(c => c.Chave == CacheDistribuidoNomes.CargosFuncoesDresEolFuncionario.Parametros(login)),
                It.IsAny<CancellationToken>()), Times.Once);

            _servicoAcessosMock.Verify(s => s.AlterarNome(login, nome), Times.Once);
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_RetornarFalseSeServicoFalhar()
        {
            // Arrange
            var login = _faker.Internet.UserName();
            var nome = _faker.Person.FullName;
            var comando = new AlterarNomeServicoAcessosCommand(login, nome);

            _servicoAcessosMock.Setup(s => s.AlterarNome(login, nome))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();

            _mediatorMock.Verify(m => m.Send(
                It.Is<RemoverCacheCommand>(c => c.Chave == CacheDistribuidoNomes.CargosFuncoesDresEolFuncionario.Parametros(login)),
                It.IsAny<CancellationToken>()), Times.Once);

            _servicoAcessosMock.Verify(s => s.AlterarNome(login, nome), Times.Once);
        }
    }
}
