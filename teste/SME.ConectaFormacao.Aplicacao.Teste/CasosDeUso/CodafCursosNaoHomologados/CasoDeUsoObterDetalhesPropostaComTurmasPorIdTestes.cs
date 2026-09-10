using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Linq;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoObterDetalhesPropostaComTurmasPorIdTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly CasoDeUsoObterDetalhesPropostaComTurmasPorId _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterDetalhesPropostaComTurmasPorIdTestes()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterDetalhesPropostaComTurmasPorId>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoExecutar_EntaoDeveRetornarResultadoComNulo()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync((PropostaComTurmasDto?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().BeNull();
            _repositorioPropostaMock.Verify(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaExistente_QuandoExecutar_EntaoDeveRetornarTurmasOrdenadasNumericamente()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            // PropostaTurmaDto é o tipo real (não TurmaPropostaDto)
            var turma1 = new PropostaTurmaDto { Nome = "Turma 10" };
            var turma2 = new PropostaTurmaDto { Nome = "Turma 2" };
            var turma3 = new PropostaTurmaDto { Nome = "Turma 1" };
            var turma4 = new PropostaTurmaDto { Nome = "Alpha" };

            // PropostaComTurmasDto usa campo Id (não PropostaId)
            var propostaDto = new PropostaComTurmasDto
            {
                Id = propostaId,
                Turmas = new List<PropostaTurmaDto> { turma1, turma2, turma3, turma4 }
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync(propostaDto);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Turmas.Should().HaveCount(4);
            // ICollection<T> não suporta indexação [] — usa ElementAt ou ToList
            var turmasOrdenadas = resultado.Dados.Turmas.ToList();
            turmasOrdenadas[0].Nome.Should().Be("Alpha");
            turmasOrdenadas[1].Nome.Should().Be("Turma 1");
            turmasOrdenadas[2].Nome.Should().Be("Turma 2");
            turmasOrdenadas[3].Nome.Should().Be("Turma 10");
            _repositorioPropostaMock.Verify(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas), Times.Once);
        }
    }
}
