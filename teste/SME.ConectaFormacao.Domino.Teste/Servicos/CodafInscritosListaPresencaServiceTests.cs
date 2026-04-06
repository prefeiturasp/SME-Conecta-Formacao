using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.Domino.Teste.Servicos
{
    public class CodafInscritosListaPresencaServiceTests
    {
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> _repositorioMock;
        private readonly CodafInscritosListaPresencaService _service;
        private readonly Faker _faker;

        public CodafInscritosListaPresencaServiceTests()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            _service = mocker.CreateInstance<CodafInscritosListaPresencaService>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmaListaDeInscritos_QuandoSalvarInscritosAsync_EntaoDeveExcluirEInserirNovamente()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();
            var inscritos = new List<CodafInscricaoListaPresenca>
            {
                new()
                {
                    Id = _faker.Random.Long(),
                    InscricaoId = _faker.Random.Long(),
                    Aprovado = _faker.Random.Bool(),
                    AtividadeObrigatorio = _faker.Random.Bool(),
                    ConceitoFinal = _faker.Random.Word(),
                    PercentualFrequencia = _faker.Random.Decimal(0, 100)
                }
            };

            // Act
            await _service.SalvarInscritosAsync(inscritos, codafListaPresencaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorListaPresencaIdAsync(codafListaPresencaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l.Count == inscritos.Count)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].CodafListaPresencaId == codafListaPresencaId)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].InscricaoId == inscritos[0].InscricaoId)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].Aprovado == inscritos[0].Aprovado)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].ConceitoFinal == inscritos[0].ConceitoFinal)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].Id == inscritos[0].Id)), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.Is<List<CodafInscricaoListaPresenca>>(l => l[0].CodafListaPresencaId == codafListaPresencaId)), Times.Once);
            inscritos[0].CodafListaPresencaId.Should().Be(codafListaPresencaId);
        }

        [Fact]
        public async Task DadoUmaListaVazia_QuandoSalvarInscritosAsync_EntaoDeveSomenteExcluir()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();
            var inscritos = new List<CodafInscricaoListaPresenca>();

            // Act
            await _service.SalvarInscritosAsync(inscritos, codafListaPresencaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorListaPresencaIdAsync(codafListaPresencaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaNulla_QuandoSalvarInscritosAsync_EntaoDeveSomenteExcluir()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long();

            // Act
            await _service.SalvarInscritosAsync(null!, codafListaPresencaId);

            // Assert
            _repositorioMock.Verify(r => r.ExcluirPorListaPresencaIdAsync(codafListaPresencaId), Times.Once);
            _repositorioMock.Verify(r => r.InserirVariosAsync(It.IsAny<List<CodafInscricaoListaPresenca>>()), Times.Never);
        }
    }
}
