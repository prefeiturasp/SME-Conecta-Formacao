using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos
{
    public class CodafCursoNaoHomologadoInscritosServiceTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologadoInscricao> _repositorioMock;
        private readonly CodafCursoNaoHomologadoInscritosService _sut;
        private readonly Faker _faker;

        public CodafCursoNaoHomologadoInscritosServiceTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologadoInscricao>();
            _sut = mocker.CreateInstance<CodafCursoNaoHomologadoInscritosService>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoInscritosValidos_QuandoChamarSalvar_EntaoDeveExcluirEInserirInscritos()
        {
            // Arrange
            var codafCursoNaoHomologadoId = _faker.Random.Long(1, 100);
            var inscritos = new List<CodafCursoNaoHomologadoInscricao>
            {
                new CodafCursoNaoHomologadoInscricao { InscricaoId = _faker.Random.Long(1, 100), Participou = true },
                new CodafCursoNaoHomologadoInscricao { InscricaoId = _faker.Random.Long(1, 100), Participou = true }
            };

            _repositorioMock.Setup(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId)).Returns(Task.CompletedTask);
            _repositorioMock.Setup(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafCursoNaoHomologadoInscricao>>())).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(inscritos, codafCursoNaoHomologadoId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<IEnumerable<CodafCursoNaoHomologadoInscricao>>(lista => 
                lista.All(i => i.CodafCursoNaoHomologadoId == codafCursoNaoHomologadoId) && 
                lista.Count() == inscritos.Count)), Times.Once);
        }

        [Fact]
        public async Task DadoInscritosVazios_QuandoChamarSalvar_EntaoDeveApenasExcluir()
        {
            // Arrange
            var codafCursoNaoHomologadoId = _faker.Random.Long(1, 100);
            var inscritos = new List<CodafCursoNaoHomologadoInscricao>();

            _repositorioMock.Setup(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId)).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(inscritos, codafCursoNaoHomologadoId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafCursoNaoHomologadoInscricao>>()), Times.Never);
        }

        [Fact]
        public async Task DadoInscritosNulos_QuandoChamarSalvar_EntaoDeveApenasExcluir()
        {
            // Arrange
            var codafCursoNaoHomologadoId = _faker.Random.Long(1, 100);

            _repositorioMock.Setup(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId)).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(null, codafCursoNaoHomologadoId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafCursoNaoHomologadoInscricao>>()), Times.Never);
        }
    }
}
