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
        private readonly Mock<IRepositorioCodafSuplementar> repositorioCodafMock;
        private readonly Mock<SME.ConectaFormacao.Infra.Dados.Relatorios.IGeradorRelatorioCodafSuplementarExcelService> geradorRelatorioMock;

        private readonly CasoDeUsoGerarRelatorioCodafSuplementar casoDeUso;

        public CasoDeUsoGerarRelatorioCodafSuplementarTestes()
        {
            repositorioMock = new Mock<IRepositorioCodafListaPresenca>();
            repositorioCodafMock = new Mock<IRepositorioCodafSuplementar>();
            geradorRelatorioMock = new Mock<SME.ConectaFormacao.Infra.Dados.Relatorios.IGeradorRelatorioCodafSuplementarExcelService>();

            casoDeUso = new CasoDeUsoGerarRelatorioCodafSuplementar(
                repositorioMock.Object,
                repositorioCodafMock.Object,
                geradorRelatorioMock.Object);
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

            repositorioCodafMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);

            geradorRelatorioMock.Verify(r =>
                r.GerarRelatorio(It.IsAny<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.RelatorioCodafDto>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_gerar_relatorio_e_atualizar_status_quando_nao_estiver_finalizado()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.AguardandoDf);
            var codafSuplementar = CriarCodafSuplementar(StatusCodafSuplementar.Aguardando);

            repositorioMock
                .Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync(lista);

            repositorioCodafMock
                .Setup(r => r.ObterPorIdCodafListaPresenca(It.IsAny<long>()))
                .ReturnsAsync(codafSuplementar);

            repositorioCodafMock
                .Setup(r => r.ObterDadosRelatorioSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync(new SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosPrincipaisRelatorioCodafDto { Participantes = new List<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosParticipanteRelatorioCodafDto>(), Retificacoes = new List<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosRetificacaoRelatorioCodafDto>() });

            geradorRelatorioMock
                .Setup(r => r.GerarRelatorio(It.IsAny<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.RelatorioCodafDto>()))
                .Returns([1, 2, 3]);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(1);

            // Assert
            Assert.True(resultado.Sucesso);

            Assert.NotNull(resultado.Dados);
            Assert.Equal(
                "CODAF_SUPLEMENTAR_123456-Turma A.xlsx",
                resultado.Dados.Nome);

            Assert.Equal(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                resultado.Dados.ContentType);

            Assert.NotNull(resultado.Dados.Stream);

            geradorRelatorioMock.Verify(r =>
                r.GerarRelatorio(It.IsAny<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.RelatorioCodafDto>()),
                Times.Once);

            repositorioCodafMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Once);
        }

        [Fact]
        public async Task Nao_deve_atualizar_quando_status_ja_for_finalizado()
        {
            // Arrange
            var lista = CriarLista(StatusCodafListaPresenca.Finalizado);
            var codafSuplementar = CriarCodafSuplementar(StatusCodafSuplementar.Finalizado);

            repositorioMock
                .Setup(r => r.ObterPorIdComPropostaEPropostaTurmaAsync(It.IsAny<long>()))
                .ReturnsAsync(lista);

            repositorioCodafMock
                .Setup(r => r.ObterPorIdCodafListaPresenca(It.IsAny<long>()))
                .ReturnsAsync(codafSuplementar);

            repositorioCodafMock
                .Setup(r => r.ObterDadosRelatorioSuplementarAsync(It.IsAny<long>()))
                .ReturnsAsync(new SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosPrincipaisRelatorioCodafDto { Participantes = new List<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosParticipanteRelatorioCodafDto>(), Retificacoes = new List<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.DadosRetificacaoRelatorioCodafDto>() });

            geradorRelatorioMock
                .Setup(r => r.GerarRelatorio(It.IsAny<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.RelatorioCodafDto>()))
                .Returns([1]);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(5);

            // Assert
            Assert.True(resultado.Sucesso);

            repositorioCodafMock.Verify(r =>
                r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);

            geradorRelatorioMock.Verify(r =>
                r.GerarRelatorio(It.IsAny<SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares.RelatorioCodafDto>()),
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

        private static CodafSuplementar CriarCodafSuplementar(StatusCodafSuplementar status)
        {
            var codafSuplementar = new CodafSuplementar(1);

            typeof(CodafSuplementar)
                .GetProperty(nameof(CodafSuplementar.Status))
                ?.SetValue(codafSuplementar, status);

            return codafSuplementar;
        }
    }
}
