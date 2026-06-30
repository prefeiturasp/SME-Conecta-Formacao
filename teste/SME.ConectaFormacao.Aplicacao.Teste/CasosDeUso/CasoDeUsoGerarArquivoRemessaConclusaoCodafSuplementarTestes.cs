using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> _repositorioCodafSuplementarMock;
        private readonly Mock<IRepositorioCodafSuplementarLogRemessaConclusao> _repositorioCodafSuplementarLogMock;
        private readonly CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar _sut;

        public CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarTestes()
        {
            var mocker = new AutoMocker();

            _repositorioCodafSuplementarMock = mocker.GetMock<IRepositorioCodafSuplementar>();
            _repositorioCodafSuplementarLogMock = mocker.GetMock<IRepositorioCodafSuplementarLogRemessaConclusao>();

            _sut = mocker.CreateInstance<CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar>();
        }
        [Fact]
        public async Task DadoCodafSuplementarSemDados_QuandoExecutarAsync_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var codafSuplementarId = faker.Random.Long(1, 100);

            _repositorioCodafSuplementarMock
                .Setup(r => r.ObterDadosRemessaConclusaoCodafSuplementarAsync(codafSuplementarId))
                .ReturnsAsync((IEnumerable<DadosConsultaParaTxtEolDto>?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(codafSuplementarId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.Dados.Should().BeNull();

            _repositorioCodafSuplementarMock.Verify(r => r.ObterPorId(It.IsAny<long>()), Times.Never);
            _repositorioCodafSuplementarLogMock.Verify(r => r.InserirAsync(It.IsAny<CodafSuplementarLogRemessaConclusao>()), Times.Never);
            _repositorioCodafSuplementarMock.Verify(r => r.Atualizar(It.IsAny<CodafSuplementar>()), Times.Never);
        }

        [Fact]
        public async Task DadoCodafSuplementarComDadosValidos_QuandoExecutarAsync_EntaoGeraArquivoERegistraLogComSucesso()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var codafSuplementarId = faker.Random.Long(1, 100);
            var numeroHomologacao = faker.Random.Long(1000, 9999);
            var nomeTurma = faker.Commerce.Department();

            var dadosBrutos = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = faker.Random.String2(7, "0123456789"),
                    CodigoCursoEol = faker.Random.Int(100, 999),
                    CodigoNivel = faker.Random.Int(1, 10),
                    DataFimCurso = faker.Date.Recent(),
                    NumeroHomologacao = numeroHomologacao,
                    HorasTotais = faker.Random.Int(10, 100),
                    NomeTurma = nomeTurma
                }
            };

            var codafSuplementarMock = new Mock<CodafSuplementar>();

            _repositorioCodafSuplementarMock
                .Setup(r => r.ObterDadosRemessaConclusaoCodafSuplementarAsync(codafSuplementarId))
                .ReturnsAsync(dadosBrutos);

            _repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorId(codafSuplementarId))
                .ReturnsAsync(codafSuplementarMock.Object);

            // Act
            var resultado = await _sut.ExecutarAsync(codafSuplementarId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.Dados.Should().NotBeNull();

            resultado.Dados!.Nome.Should().StartWith("HOM");
            resultado.Dados.Nome.Should().Contain(numeroHomologacao.ToString());
            resultado.Dados.ContentType.Should().Be("application/octet-stream");
            resultado.Dados.Stream.Should().NotBeNull();
            resultado.Dados.Stream.Length.Should().BeGreaterThan(0);

            _repositorioCodafSuplementarLogMock.Verify(r => r.InserirAsync(It.Is<CodafSuplementarLogRemessaConclusao>(log =>
                log.CodafSuplementarId == codafSuplementarId &&
                log.QuantidadeRegistros == dadosBrutos.Count &&
                log.NomeArquivoGerado == resultado.Dados.Nome &&
                !string.IsNullOrWhiteSpace(log.HashArquivo)
            )), Times.Once);

            _repositorioCodafSuplementarMock.Verify(r => r.Atualizar(codafSuplementarMock.Object), Times.Once);
        }
    }
}
