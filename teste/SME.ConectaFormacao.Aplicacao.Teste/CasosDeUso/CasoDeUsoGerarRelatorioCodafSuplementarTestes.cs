using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> _repositorioCodafSuplementarMock;
        private readonly Mock<IGeradorRelatorioCodafExcelService> _geradorRelatorioMock;
        private readonly CasoDeUsoGerarRelatorioCodafSuplementar _sut;
        private readonly Faker _faker;

        public CasoDeUsoGerarRelatorioCodafSuplementarTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafSuplementarMock = mocker.GetMock<IRepositorioCodafSuplementar>();
            _geradorRelatorioMock = mocker.GetMock<IGeradorRelatorioCodafExcelService>();
            
            _sut = mocker.CreateInstance<CasoDeUsoGerarRelatorioCodafSuplementar>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoExistente_QuandoChamarExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            _repositorioCodafSuplementarMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(codafId))
                .ReturnsAsync((CodafSuplementar?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Nenhuma informação encontrada para o codaf informado.");
        }

        [Fact]
        public async Task DadoDadosRelatorioNaoExistente_QuandoChamarExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var codafSuplementar = new CodafSuplementar(codafId);
            
            _repositorioCodafSuplementarMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(codafId))
                .ReturnsAsync(codafSuplementar);

            _repositorioCodafSuplementarMock.Setup(r => r.ObterDadosRelatorioSuplementarAsync(codafId))
                .ReturnsAsync((DadosPrincipaisRelatorioCodafDto?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Nenhuma informação encontrada para o codaf informado.");
        }

        [Fact]
        public async Task DadoDadosValidos_EStatusNaoFinalizado_QuandoChamarExecutar_EntaoDeveGerarRelatorioEAtualizarStatus()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var codafSuplementar = new CodafSuplementar(codafId);
            typeof(CodafSuplementar).GetProperty("Status")?.SetValue(codafSuplementar, StatusCodafSuplementar.Aguardando);
            
            var dadosRelatorio = new DadosPrincipaisRelatorioCodafDto
            {
                DataCodaf = new DateTime(2023, 10, 1),
                NumeroHomologacao = 12345,
                NomeTurma = "Turma A"
            };

            var bytesRelatorio = new byte[] { 1, 2, 3 };

            _repositorioCodafSuplementarMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(codafId))
                .ReturnsAsync(codafSuplementar);

            _repositorioCodafSuplementarMock.Setup(r => r.ObterDadosRelatorioSuplementarAsync(codafId))
                .ReturnsAsync(dadosRelatorio);

            _geradorRelatorioMock.Setup(g => g.GerarRelatorio(dadosRelatorio, true))
                .Returns(bytesRelatorio);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            resultado.Dados.Nome.Should().Be("CODAF_SUPLEMENTAR_12345-Turma A.xlsx");
            
            dadosRelatorio.ObservacaoCodafSuplementar.Should().Be("Documento suplementar do arquivo gerado em 01/10/2023");
            
            codafSuplementar.Status.Should().Be(StatusCodafSuplementar.Finalizado);
            _repositorioCodafSuplementarMock.Verify(r => r.Atualizar(codafSuplementar), Times.Once);
        }

        [Fact]
        public async Task DadoDadosValidos_EStatusJaFinalizado_QuandoChamarExecutar_EntaoDeveGerarRelatorioMasNaoAtualizarStatus()
        {
            // Arrange
            var codafId = _faker.Random.Long(1, 1000);
            var codafSuplementar = new CodafSuplementar(codafId);
            typeof(CodafSuplementar).GetProperty("Status")?.SetValue(codafSuplementar, StatusCodafSuplementar.Finalizado);
            
            var dadosRelatorio = new DadosPrincipaisRelatorioCodafDto
            {
                DataCodaf = new DateTime(2023, 10, 1),
                NumeroHomologacao = 12345,
                NomeTurma = "Turma A"
            };

            var bytesRelatorio = new byte[] { 1, 2, 3 };

            _repositorioCodafSuplementarMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(codafId))
                .ReturnsAsync(codafSuplementar);

            _repositorioCodafSuplementarMock.Setup(r => r.ObterDadosRelatorioSuplementarAsync(codafId))
                .ReturnsAsync(dadosRelatorio);

            _geradorRelatorioMock.Setup(g => g.GerarRelatorio(dadosRelatorio, true))
                .Returns(bytesRelatorio);

            // Act
            var resultado = await _sut.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            
            _repositorioCodafSuplementarMock.Verify(r => r.Atualizar(codafSuplementar), Times.Never);
        }
    }
}
