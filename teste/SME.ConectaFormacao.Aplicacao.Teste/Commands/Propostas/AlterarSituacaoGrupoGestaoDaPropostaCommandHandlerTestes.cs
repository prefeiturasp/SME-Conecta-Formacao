using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class AlterarSituacaoGrupoGestaoDaPropostaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoGrupoGestaoDaPropostaCommandHandler _handler;

        public AlterarSituacaoGrupoGestaoDaPropostaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _handler = mocker.CreateInstance<AlterarSituacaoGrupoGestaoDaPropostaCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveAtualizarSituacaoERetornarTrue()
        {
            // Arrange
            var comando = new AlterarSituacaoGrupoGestaoDaPropostaCommand(
                _faker.Random.Long(1, 1000), 
                _faker.PickRandom<SituacaoProposta>(), 
                _faker.Random.Long(1, 1000));

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioPropostaMock.Verify(r => r.AtualizarSituacaoGrupoGestao(comando.Id, comando.SituacaoProposta, comando.GrupoGestaoId), Times.Once);
        }
    }
}
