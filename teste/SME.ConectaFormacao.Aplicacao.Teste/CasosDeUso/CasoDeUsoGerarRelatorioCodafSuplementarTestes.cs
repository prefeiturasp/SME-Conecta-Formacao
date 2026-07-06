using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> repositorioMock;
        private readonly Mock<IServicoRelatorio> servicoRelatorioMock;

        private readonly CasoDeUsoGerarRelatorioCodafSuplementar casoDeUso;

        public CasoDeUsoGerarRelatorioCodafSuplementarTestes()
        {
            repositorioMock = new Mock<IRepositorioCodafListaPresenca>();
            servicoRelatorioMock = new Mock<IServicoRelatorio>();

            casoDeUso = new CasoDeUsoGerarRelatorioCodafSuplementar(
                repositorioMock.Object,
                servicoRelatorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_nao_encontrado_quando_lista_nao_existir()
        {
            // Arrange
            repositorioMock
                .Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync((CodafListaPresenca)null!);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(10);

            // Assert
            Assert.False(resultado.Sucesso);

            repositorioMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafListaPresenca>()),
                Times.Never);

            servicoRelatorioMock.Verify(r =>
                r.GerarRelatorioCodafSuplementarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_gerar_relatorio_e_atualizar_status_quando_nao_estiver_finalizado()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf);

            repositorioMock
                .Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync(lista);

            servicoRelatorioMock
                .Setup(r => r.GerarRelatorioCodafSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync([1, 2, 3]);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(1);

            // Assert
            Assert.True(resultado.Sucesso);

            Assert.NotNull(resultado.Dados);
            Assert.Equal(
                "CODAF_123456-Turma A.xlsx",
                resultado.Dados.Nome);

            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                resultado.Dados.ContentType);

            Assert.NotNull(resultado.Dados.Stream);

            servicoRelatorioMock.Verify(r =>
                r.GerarRelatorioCodafSuplementarAsync(1),
                Times.Once);

            repositorioMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafListaPresenca>()),
                Times.Once);
        }

        [Fact]
        public async Task Nao_deve_atualizar_quando_status_ja_for_finalizado()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.Finalizado);

            repositorioMock
                .Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync(lista);

            servicoRelatorioMock
                .Setup(r => r.GerarRelatorioCodafSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync([1]);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(5);

            // Assert
            Assert.True(resultado.Sucesso);

            repositorioMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafListaPresenca>()),
                Times.Never);

            servicoRelatorioMock.Verify(r =>
                r.GerarRelatorioCodafSuplementarAsync(5),
                Times.Once);
        }

        private static CodafListaPresenca CriarLista(StatusCodafListaPresenca status)
        {
            var lista = new CodafListaPresenca
            {
                Proposta = new Proposta
                {
                    NumeroHomologacao = 123456
                },
                PropostaTurma = new PropostaTurma
                {
                    Nome = "Turma A"
                }
            };

            typeof(CodafListaPresenca)
                .GetProperty(nameof(CodafListaPresenca.Status))
                ?.SetValue(lista, status);

            return lista;
        }
    }
}
