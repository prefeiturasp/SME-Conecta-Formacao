using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterPropostaTurmaComCodafTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IServicoPeriodoEncontroProposta> _servicoPeriodoEncontroPropostaMock;
        private readonly CasoDeUsoObterPropostaTurmaComCodaf _sut;

        public CasoDeUsoObterPropostaTurmaComCodafTestes()
        {
            var mocker = new AutoMocker();

            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _servicoPeriodoEncontroPropostaMock = mocker.GetMock<IServicoPeriodoEncontroProposta>();

            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaTurmaComCodaf>();
        }

        [Fact]
        public async Task DadoPropostaComTurmasECodaf_QuandoExecutarAsync_EntaoRetornaListaMapeadaComCodafId()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var propostaId = faker.Random.Long(1, 100);
            var turmaId = faker.Random.Long(1, 100);
            var codafId = faker.Random.Long(1, 100);
            var periodo = $" - {faker.Date.Future():dd/MM/yyyy} a {faker.Date.Future():dd/MM/yyyy}";

            var codaf = new CodafListaPresenca(propostaId, turmaId, StatusCodafListaPresenca.Iniciado)
            {
                Id = codafId
            };

            var turma = new PropostaTurma
            {
                Id = turmaId,
                Nome = faker.Commerce.Department(),
                CodafListaPresenca = codaf
            };

            var turmas = new List<PropostaTurma> { turma };

            _repositorioPropostaMock
                .Setup(r => r.ObterTurmasComCodafAsync(propostaId))
                .ReturnsAsync(turmas);

            _servicoPeriodoEncontroPropostaMock
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(turmaId))
                .ReturnsAsync(periodo);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNullOrEmpty();
            resultado.Dados.Should().HaveCount(1);

            var turmaDto = resultado.Dados!.First();
            turmaDto.Id.Should().Be(turma.Id);
            turmaDto.Descricao.Should().Be(turma.Nome + periodo);
            turmaDto.CodafId.Should().Be(codaf.Id);

            _repositorioPropostaMock.Verify(r => r.ObterTurmasComCodafAsync(propostaId), Times.Once);
            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(turmaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComTurmasSemCodaf_QuandoExecutarAsync_EntaoRetornaListaComCodafIdZero()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var propostaId = faker.Random.Long(1, 100);
            var turmaId = faker.Random.Long(1, 100);
            var periodo = $" - {faker.Date.Future():dd/MM/yyyy}";

            var turma = new PropostaTurma
            {
                Id = turmaId,
                Nome = faker.Commerce.Department(),
                CodafListaPresenca = null
            };

            var turmas = new List<PropostaTurma> { turma };

            _repositorioPropostaMock
                .Setup(r => r.ObterTurmasComCodafAsync(propostaId))
                .ReturnsAsync(turmas);

            _servicoPeriodoEncontroPropostaMock
                .Setup(s => s.ObterPeriodoEncontrosTurmaAsync(turmaId))
                .ReturnsAsync(periodo);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNullOrEmpty();
            resultado.Dados.Should().HaveCount(1);

            var turmaDto = resultado.Dados!.First();
            turmaDto.Id.Should().Be(turma.Id);
            turmaDto.Descricao.Should().Be(turma.Nome + periodo);
            turmaDto.CodafId.Should().Be(0);

            _repositorioPropostaMock.Verify(r => r.ObterTurmasComCodafAsync(propostaId), Times.Once);
            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(turmaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaSemTurmas_QuandoExecutarAsync_EntaoRetornaListaVazia()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var propostaId = faker.Random.Long(1, 100);

            _repositorioPropostaMock
                .Setup(r => r.ObterTurmasComCodafAsync(propostaId))
                .ReturnsAsync([]);

            // Act
            var resultado = await _sut.ExecutarAsync(propostaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Should().BeEmpty();

            _repositorioPropostaMock.Verify(r => r.ObterTurmasComCodafAsync(propostaId), Times.Once);
            _servicoPeriodoEncontroPropostaMock.Verify(s => s.ObterPeriodoEncontrosTurmaAsync(It.IsAny<long>()), Times.Never);
        }
    }
}
