using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarNumeroHomologacaoProposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class SalvarNumeroHomologacaoPropostaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly SalvarNumeroHomologacaoPropostaCommandHandler _sut;

        public SalvarNumeroHomologacaoPropostaCommandHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioPropostaMock = autoMocker.GetMock<IRepositorioProposta>();
            _sut = autoMocker.CreateInstance<SalvarNumeroHomologacaoPropostaCommandHandler>();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoLinhasAfetadasMaiorQueZero_EntaoRetornaTrue()
        {
            // Arrange
            const long propostaId = 10;
            const long numeroHomologacao = 54321;
            var command = new SalvarNumeroHomologacaoPropostaCommand(propostaId, numeroHomologacao);

            _repositorioPropostaMock
                .Setup(r => r.AtualizarNumeroHomologacao(propostaId, numeroHomologacao))
                .ReturnsAsync(1);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioPropostaMock.Verify(r => r.AtualizarNumeroHomologacao(propostaId, numeroHomologacao), Times.Once);
        }

        [Fact]
        public async Task DadoComandoValido_QuandoLinhasAfetadasZero_EntaoRetornaFalse()
        {
            // Arrange
            const long propostaId = 10;
            const long numeroHomologacao = 54321;
            var command = new SalvarNumeroHomologacaoPropostaCommand(propostaId, numeroHomologacao);

            _repositorioPropostaMock
                .Setup(r => r.AtualizarNumeroHomologacao(propostaId, numeroHomologacao))
                .ReturnsAsync(0);

            // Act
            var resultado = await _sut.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _repositorioPropostaMock.Verify(r => r.AtualizarNumeroHomologacao(propostaId, numeroHomologacao), Times.Once);
        }
    }
}
