using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> repositorio;
        private readonly Mock<IRepositorioCodafSuplementarLogRemessaConclusao> repositorioLog;

        private readonly CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar casoDeUso;

        public CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarTestes()
        {
            repositorio = new Mock<IRepositorioCodafSuplementar>();
            repositorioLog = new Mock<IRepositorioCodafSuplementarLogRemessaConclusao>();

            casoDeUso = new CasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar(
                repositorio.Object,
                repositorioLog.Object);
        }

        [Fact]
        public async Task Deve_retornar_nao_encontrado_quando_nao_existirem_dados()
        {
            repositorio.Setup(x =>
                    x.ObterDadosRemessaConclusaoCodafSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync((IEnumerable<DadosConsultaParaTxtEolDto>)null!);

            var resultado = await casoDeUso.ExecutarAsync(1);

            Assert.False(resultado.Sucesso);

            repositorioLog.Verify(
                x => x.InserirAsync(It.IsAny<Dominio.Entidades.CodafSuplementarLogRemessaConclusao>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_gerar_arquivo_e_registrar_log_utilizando_horas_totais()
        {
            var dados = new List<DadosConsultaParaTxtEolDto>
        {
            new()
            {
                RegistroFuncional = "123456",
                CodigoCursoEol = 987,
                CodigoNivel = 5,
                NumeroHomologacao = 100,
                NomeTurma = "Turma Á/2025",
                HorasTotais = 40,
                DataFimCurso = new DateTime(2025,12,20)
            }
        };

            repositorio.Setup(x =>
                    x.ObterDadosRemessaConclusaoCodafSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync(dados);

            var resultado = await casoDeUso.ExecutarAsync(10);

            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);

            Assert.Equal("application/octet-stream", resultado.Dados.ContentType);
            Assert.StartsWith("HOM100", resultado.Dados.Nome);

            using var reader = new StreamReader(resultado.Dados.Stream);

            var conteudo = await reader.ReadToEndAsync();

            Assert.Contains("123456", conteudo);

            repositorioLog.Verify(x =>
                x.InserirAsync(It.Is<Dominio.Entidades.CodafSuplementarLogRemessaConclusao>(
                    l =>
                        l.CodafSuplementarId == 10 &&
                        l.QuantidadeRegistros == 1 &&
                        !string.IsNullOrWhiteSpace(l.HashArquivo) &&
                        l.NomeArquivoGerado.StartsWith("HOM100"))),
                Times.Once);
        }

        [Fact]
        public async Task Deve_utilizar_carga_horaria_quando_horas_totais_for_nulo()
        {
            var dados = new List<DadosConsultaParaTxtEolDto>
        {
            new()
            {
                RegistroFuncional = "999",
                CodigoCursoEol = 10,
                CodigoNivel = 1,
                NumeroHomologacao = 200,
                NomeTurma = "Turma Teste",
                HorasTotais = null,
                CargaHorariaTotalOutra = "08:30",
                DataFimCurso = null
            }
        };

            repositorio.Setup(x =>
                    x.ObterDadosRemessaConclusaoCodafSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync(dados);

            var resultado = await casoDeUso.ExecutarAsync(5);

            Assert.True(resultado.Sucesso);

            using var reader = new StreamReader(resultado.Dados!.Stream);

            var conteudo = await reader.ReadToEndAsync();

            Assert.Contains("08", conteudo);

            repositorioLog.Verify(x =>
                x.InserirAsync(It.IsAny<Dominio.Entidades.CodafSuplementarLogRemessaConclusao>()),
                Times.Once);
        }
    }
}
