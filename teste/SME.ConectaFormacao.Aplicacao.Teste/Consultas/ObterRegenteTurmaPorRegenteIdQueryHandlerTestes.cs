using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegenteTurmaPorRegenteId;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterRegenteTurmaPorRegenteIdQueryHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ObterRegenteTurmaPorRegenteIdQueryHandler _sut;

        public ObterRegenteTurmaPorRegenteIdQueryHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<ObterRegenteTurmaPorRegenteIdQueryHandler>();
        }

        [Fact]
        public async Task DadoRequestValido_QuandoExecutar_EntaoRetornaTurmasDoRegente()
        {
            // Arrange
            var query = new ObterRegenteTurmaPorRegenteIdQuery(1);

            var turmas = new List<PropostaRegenteTurma>
            {
                new() { Id = 1, PropostaRegenteId = 1, TurmaId = 10 }
            };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegenteTurmasPorRegenteId(1))
                .ReturnsAsync(turmas);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(turmas);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.ObterRegenteTurmasPorRegenteId(1), Times.Once);
        }
    }
}
