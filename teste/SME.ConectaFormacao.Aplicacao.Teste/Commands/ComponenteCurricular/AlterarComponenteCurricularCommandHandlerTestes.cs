using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ComponenteCurricular
{
    public class AlterarComponenteCurricularCommandHandlerTestes
    {
        private readonly Mock<IRepositorioComponenteCurricular> _repositorioComponenteCurricularMock;
        private readonly AlterarComponenteCurricularCommandHandler _handler;

        public AlterarComponenteCurricularCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioComponenteCurricularMock = mocker.GetMock<IRepositorioComponenteCurricular>();
            _handler = mocker.CreateInstance<AlterarComponenteCurricularCommandHandler>();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarMetodo_EntaoRetornaResultadoEsperado()
        {
            // Arrange
            var componenteCurricular = new Dominio.Entidades.ComponenteCurricular();
            var comando = new AlterarComponenteCurricularCommand(componenteCurricular);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioComponenteCurricularMock.Verify(r => r.Atualizar(componenteCurricular), Times.Once);
        }
    }
}
