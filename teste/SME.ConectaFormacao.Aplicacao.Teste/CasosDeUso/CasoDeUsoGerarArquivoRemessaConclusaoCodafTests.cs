using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodafTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioMock;
        private readonly CasoDeUsoGerarArquivoRemessaConclusaoCodaf _casoDeUso;

        public CasoDeUsoGerarArquivoRemessaConclusaoCodafTests()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoGerarArquivoRemessaConclusaoCodaf>();
        }

        [Fact]
        public async Task DadoQueNaoExistemDadosParaOIdInformado_QuandoExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            long codafId = 1;

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoQueExistemDadosValidos_QuandoExecutar_EntaoDeveGerarArquivoENomeCorretamente()
        {
            // Arrange
            long codafId = 10;
            var dadosBanco = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = "1234567",
                    CodigoCursoEol = 98765,
                    CodigoNivel = 1,
                    DataFimCurso = new DateTime(2023, 10, 15, 0, 0, 0, DateTimeKind.Utc),
                    NumeroHomologacao = 2023001,
                    NomeTurma = "Turma A - Avançada!",
                    HorasTotais = 20,
                    CargaHorariaTotalOutra = null
                }
            };

            _repositorioMock.Setup(x => x.ObterDadosRemessaConclusaoCodafAsync(codafId))
                .ReturnsAsync(dadosBanco);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Stream.Should().NotBeNull();
            resultado.Dados.Stream!.Length.Should().BeGreaterThan(0);
            resultado.Dados.Stream.Position.Should().Be(0);

            resultado.Dados.Nome.Should().StartWith("HOM2023001");
            resultado.Dados.Nome.Should().EndWith(".txt");
            resultado.Dados.Nome.Should().NotContain("!");
        }

        [Fact]
        public async Task DadoQueOsDadosEstaoPreenchidos_QuandoExecutar_EntaoOConteudoDoArquivoDeveEstarFormatadoCorretamente()
        {
            // Arrange
            long codafId = 5;
            var dadosBanco = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = "7777777",
                    CodigoCursoEol = 54321,
                    CodigoNivel = 2,
                    DataFimCurso = new DateTime(2023, 12, 01, 0, 0, 0, DateTimeKind.Utc),
                    NumeroHomologacao = 999,
                    NomeTurma = "Teste",
                    HorasTotais = 8,
                    CargaHorariaTotalOutra = null
                }
            };

            _repositorioMock.Setup(x => x.ObterDadosRemessaConclusaoCodafAsync(codafId))
                .ReturnsAsync(dadosBanco);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            using var reader = new StreamReader(resultado.Dados.Stream!, Encoding.UTF8);
            var conteudo = await reader.ReadToEndAsync();

            // Layout esperado: RF|CodCurso|Data|Nivel(00)|HOM+Num|Carga(00)
            // 7777777|54321|01/12/2023|02|HOM999|08
            var linhaEsperada = "7777777|54321|01/12/2023|02|HOM999|08";

            conteudo.Trim().Should().Be(linhaEsperada);
        }

        [Fact]
        public async Task DadoQueHorasTotaisSejaNulo_QuandoExecutar_EntaoDeveCalcularCargaHorariaPeloTexto()
        {
            // Arrange
            long codafId = 8;
            var dadosBanco = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = "111",
                    CodigoCursoEol = 222,
                    CodigoNivel = 1,
                    DataFimCurso = DateTime.Now,
                    NumeroHomologacao = 1,
                    NomeTurma = "T",
                    HorasTotais = null,
                    CargaHorariaTotalOutra = "04:30"
                }
            };

            _repositorioMock.Setup(x => x.ObterDadosRemessaConclusaoCodafAsync(codafId))
                .ReturnsAsync(dadosBanco);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId);

            // Assert
            resultado.Dados.Should().NotBeNull();
            using var reader = new StreamReader(resultado.Dados.Stream!, Encoding.UTF8);
            var conteudo = await reader.ReadToEndAsync();

            // Verifica se a última coluna é "04"
            conteudo.Trim().Should().EndWith("|04");
        }

        [Fact]
        public async Task DadoQueHorasTotaisSejaNuloETextoSejaInvalido_QuandoExecutar_EntaoDeveRetornarZero()
        {
            // Arrange
            long codafId = 9;
            var dadosBanco = new List<DadosConsultaParaTxtEolDto>
            {
                new()
                {
                    RegistroFuncional = "111",
                    CodigoCursoEol = 222,
                    CodigoNivel = 1,
                    DataFimCurso = DateTime.Now,
                    NumeroHomologacao = 1,
                    NomeTurma = "T",
                    HorasTotais = null,
                    CargaHorariaTotalOutra = "TextoInvalido"
                }
            };

            _repositorioMock.Setup(x => x.ObterDadosRemessaConclusaoCodafAsync(codafId))
                .ReturnsAsync(dadosBanco);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafId);

            // Assert
            resultado.Dados.Should().NotBeNull();
            using var reader = new StreamReader(resultado.Dados.Stream!, Encoding.UTF8);
            var conteudo = await reader.ReadToEndAsync();

            // Verifica se a última coluna é "00" (zero formatado)
            conteudo.Trim().Should().EndWith("|00");
        }
    }
}
