using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos
{
    public class CodafConferenciaInscritosServiceTestes
    {
        private readonly Mock<IRepositorioCodafConferenciaInscricao> _repositorioMock;
        private readonly CodafConferenciaInscritosService _sut;
        private readonly Faker _faker;

        public CodafConferenciaInscritosServiceTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferenciaInscricao>();
            _sut = mocker.CreateInstance<CodafConferenciaInscritosService>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoInscritosValidos_QuandoChamarSalvar_EntaoDeveExcluirEInserirInscritos()
        {
            // Arrange
            var codafConferenciaId = _faker.Random.Long(1, 100);
            var inscritos = new List<CodafConferenciaInscricao>
            {
                new() { InscricaoId = _faker.Random.Long(1, 100), Participou = true },
                new() { InscricaoId = _faker.Random.Long(1, 100), Participou = false }
            };

            _repositorioMock.Setup(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId)).Returns(Task.CompletedTask);
            _repositorioMock.Setup(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafConferenciaInscricao>>())).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(inscritos, codafConferenciaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<IEnumerable<CodafConferenciaInscricao>>(lista =>
                lista.All(i => i.CodafConferenciaId == codafConferenciaId) &&
                lista.Count() == inscritos.Count)), Times.Once);
        }

        [Fact]
        public async Task DadoInscritosVazios_QuandoChamarSalvar_EntaoDeveApenasExcluir()
        {
            // Arrange
            var codafConferenciaId = _faker.Random.Long(1, 100);
            var inscritos = new List<CodafConferenciaInscricao>();

            _repositorioMock.Setup(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId)).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(inscritos, codafConferenciaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafConferenciaInscricao>>()), Times.Never);
        }

        [Fact]
        public async Task DadoInscritosNulos_QuandoChamarSalvar_EntaoDeveApenasExcluir()
        {
            // Arrange
            var codafConferenciaId = _faker.Random.Long(1, 100);

            _repositorioMock.Setup(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId)).Returns(Task.CompletedTask);

            // Act
            await _sut.SalvarInscritosAsync(null!, codafConferenciaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorCodafConferenciaIdAsync(codafConferenciaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<IEnumerable<CodafConferenciaInscricao>>()), Times.Never);
        }
    }
}
