using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Linq;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafCursosNaoHomologados
{
    public class CasoDeUsoObterDetalhesPropostaComTurmasPorIdTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IServicoPeriodoEncontroProposta> _servicoPeriodoEncontroPropostaMock;
        private readonly CasoDeUsoObterDetalhesPropostaComTurmasPorId _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterDetalhesPropostaComTurmasPorIdTestes()
        {
            _mocker = new AutoMocker();
            _repositorioPropostaMock = _mocker.GetMock<IRepositorioProposta>();
            _servicoPeriodoEncontroPropostaMock = _mocker.GetMock<IServicoPeriodoEncontroProposta>();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoObterDetalhesPropostaComTurmasPorId>();
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

            var turma1 = new PropostaTurmaDto { Id = 1, Nome = "Turma 10" };
            var turma2 = new PropostaTurmaDto { Id = 2, Nome = "Turma 2" };
            var turma3 = new PropostaTurmaDto { Id = 3, Nome = "Turma 1" };
            var turma4 = new PropostaTurmaDto { Id = 4, Nome = "Alpha" };

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
            var turmasOrdenadas = resultado.Dados.Turmas.ToList();
            turmasOrdenadas[0].Nome.Should().Be("Alpha");
            turmasOrdenadas[1].Nome.Should().Be("Turma 1");
            turmasOrdenadas[2].Nome.Should().Be("Turma 2");
            turmasOrdenadas[3].Nome.Should().Be("Turma 10");
            _repositorioPropostaMock.Verify(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComTurmasDuplicadas_QuandoExecutar_EntaoDeveDeduplicarTurmas()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            var turma1A = new PropostaTurmaDto { Id = 100, Nome = "Turma 1" };
            var turma1B = new PropostaTurmaDto { Id = 100, Nome = "Turma 1" };
            var turma2 = new PropostaTurmaDto { Id = 200, Nome = "Turma 2" };

            var propostaDto = new PropostaComTurmasDto
            {
                Id = propostaId,
                Turmas = new List<PropostaTurmaDto> { turma1A, turma1B, turma2 }
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync(propostaDto);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Turmas.Should().HaveCount(2);
            resultado.Dados.Turmas.Select(t => t.Id).Should().ContainInOrder(100, 200);
        }

        [Fact]
        public async Task DadoPropostaComTurmas_QuandoExecutar_EntaoDeveFormatarDescricaoComPeriodoDeEncontros()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            var turma1 = new PropostaTurmaDto { Id = 10, Nome = "Turma 1" };
            var turma2 = new PropostaTurmaDto { Id = 20, Nome = "Turma 2" };

            var propostaDto = new PropostaComTurmasDto
            {
                Id = propostaId,
                Turmas = new List<PropostaTurmaDto> { turma1, turma2 }
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync(propostaDto);

            _servicoPeriodoEncontroPropostaMock
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(10))
                .ReturnsAsync(" 01/03/2026 até 10/03/2026");

            _servicoPeriodoEncontroPropostaMock
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(20))
                .ReturnsAsync(" 15/03/2026");

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            var turmas = resultado.Dados!.Turmas.ToList();
            turmas[0].Descricao.Should().Be("Turma 1 01/03/2026 até 10/03/2026");
            turmas[0].Nome.Should().Be("Turma 1");
            turmas[1].Descricao.Should().Be("Turma 2 15/03/2026");
            turmas[1].Nome.Should().Be("Turma 2");

            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(10), Times.Once);
            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(20), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComTurmasSemEncontros_QuandoExecutar_EntaoDescricaoDeveSerApenasNome()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            var turma1 = new PropostaTurmaDto { Id = 10, Nome = "Turma Sem Encontros" };

            var propostaDto = new PropostaComTurmasDto
            {
                Id = propostaId,
                Turmas = new List<PropostaTurmaDto> { turma1 }
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync(propostaDto);

            _servicoPeriodoEncontroPropostaMock
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(10))
                .ReturnsAsync(string.Empty);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            var turma = resultado.Dados!.Turmas.First();
            turma.Descricao.Should().Be("Turma Sem Encontros");
            turma.Nome.Should().Be("Turma Sem Encontros");
        }

        [Fact]
        public async Task DadoPropostaSemTurmas_QuandoExecutar_EntaoDeveRetornarPropostaComTurmasVazia()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacoesHomologadas = _faker.Random.Bool();

            var propostaDto = new PropostaComTurmasDto
            {
                Id = propostaId,
                Turmas = new List<PropostaTurmaDto>()
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterDetalhesPropostaComTurmasPorIdAsync(propostaId, formacoesHomologadas))
                .ReturnsAsync(propostaDto);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(propostaId, formacoesHomologadas);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Turmas.Should().BeEmpty();
            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(It.IsAny<long>()), Times.Never);
        }
    }
}
