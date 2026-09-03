using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class AlterarSituacaoDaPropostaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoDaPropostaCommandHandler _handler;

        public AlterarSituacaoDaPropostaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _handler = mocker.CreateInstance<AlterarSituacaoDaPropostaCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveAtualizarSituacaoERetornarTrue()
        {
            // Arrange
            var comando = new AlterarSituacaoDaPropostaCommand(_faker.Random.Long(1, 1000), SituacaoProposta.Aprovada);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioPropostaMock.Verify(r => r.AtualizarSituacao(comando.Id, comando.SituacaoProposta), Times.Once);
        }
    }
}
