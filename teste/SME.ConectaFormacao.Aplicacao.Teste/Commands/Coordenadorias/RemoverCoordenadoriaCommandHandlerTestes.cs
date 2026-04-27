using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Coordenadorias
{
    public class RemoverCoordenadoriaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioCoordenadoria> _repositorioCoordenadoria;
        private readonly RemoverCoordenadoriaCommandHandler _sut;

        public RemoverCoordenadoriaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioCoordenadoria = mocker.GetMock<IRepositorioCoordenadoria>();
            _sut = mocker.CreateInstance<RemoverCoordenadoriaCommandHandler>();
        }

        [Fact]
        public async Task DadoIdValido_QuandoRemover_DeveExcluirCoordenadoria()
        {
            // Arrange
            var command = new RemoverCoordenadoriaCommand(1);
            _repositorioCoordenadoria
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Coordenadoria() { Id = command.Id, Nome = "Coordenadoria Teste", Sigla = "CT" });

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            _repositorioCoordenadoria
                .Verify(r => r.Atualizar(It.Is<Coordenadoria>(c =>
                                                                c.Id == command.Id &&
                                                                c.Excluido))
                , Times.Once);
            result.Sucesso.Should().BeTrue();
        }

        [Fact]
        public async Task DadoIdInvalido_QuandoRemover_DeveRetornarErro()
        {
            // Arrange
            var command = new RemoverCoordenadoriaCommand(1);

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            _repositorioCoordenadoria.Verify(r => r.Atualizar(It.IsAny<Coordenadoria>()), Times.Never);
            result.Sucesso.Should().BeFalse();
            result.MensagensErro.Should().Contain("Coordenadoria não encontrada.");
        }
    }
}
