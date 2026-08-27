using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.AnoTurma
{
    public class AlterarAnoTurmaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioAnoTurma> _repositorioAnoTurmaMock;
        private readonly Faker _faker;
        private readonly AlterarAnoTurmaCommandHandler _handler;

        public AlterarAnoTurmaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioAnoTurmaMock = mocker.GetMock<IRepositorioAnoTurma>();
            _handler = mocker.CreateInstance<AlterarAnoTurmaCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_EntaoDeveAtualizarERetornarTrue()
        {
            // Arrange
            var anoTurma = new Dominio.Entidades.AnoTurma
            {
                Id = _faker.Random.Long(1, 1000),
                AnoLetivo = (short)_faker.Random.Int(2020, 2030),
                Descricao = _faker.Random.String2(10),
                CodigoSerieEnsino = _faker.Random.Long(1, 1000),
                CodigoEOL = _faker.Random.Long(1, 1000).ToString()
            };

            var comando = new AlterarAnoTurmaCommand(anoTurma);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioAnoTurmaMock.Verify(r => r.Atualizar(It.Is<Dominio.Entidades.AnoTurma>(a => a == anoTurma)), Times.Once);
        }
    }
}
