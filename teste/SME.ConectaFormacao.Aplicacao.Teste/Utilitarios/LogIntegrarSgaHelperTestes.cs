using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Utilitarios
{
    public class LogIntegrarSgaHelperTestes
    {
        private readonly Mock<IServicoLogs> _servicoLogsMock = new();

        [Fact]
        public async Task DadoMudancaDeTrueParaFalse_QuandoRegistrarLogSga_EntaoDeveEnviarComNivelAlerta()
        {
            // Arrange
            string? mensagemCapturada = null;
            LogNivel? nivelCapturado = null;
            string? observacaoCapturada = null;

            _servicoLogsMock
                .Setup(s => s.Enviar(It.IsAny<string>(), It.IsAny<LogContexto>(), It.IsAny<LogNivel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, LogContexto, LogNivel, string, string?>((m, c, n, o, r) =>
                {
                    mensagemCapturada = m;
                    nivelCapturado = n;
                    observacaoCapturada = o;
                })
                .Returns(Task.CompletedTask);

            // Act
            await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                _servicoLogsMock.Object,
                operacao: "AlterarProposta",
                propostaId: 123,
                valorBancoAnterior: true,
                valorRecebidoDto: false,
                valorFinalPersistido: false,
                nomeFormacao: "Curso Teste",
                areaPromotoraId: 10,
                situacao: "Cadastrada",
                usuario: "diegomoreno"
            );

            // Assert
            mensagemCapturada.Should().NotBeNull();
            mensagemCapturada.Should().StartWith("[INTEGRA_SGA]");
            mensagemCapturada.Should().Contain("PropostaId: 123");
            mensagemCapturada.Should().Contain("Banco: TRUE");
            mensagemCapturada.Should().Contain("DTO: FALSE");
            mensagemCapturada.Should().Contain("Final: FALSE");
            nivelCapturado.Should().Be(LogNivel.Alerta);
            observacaoCapturada.Should().Contain("\"mudouDeTrueParaFalse\":true");
            observacaoCapturada.Should().Contain("\"propostaId\":123");
        }

        [Fact]
        public async Task DadoDtoNuloComBancoTrue_QuandoRegistrarLogSga_EntaoDevePreservarENivelInformacao()
        {
            // Arrange
            string? mensagemCapturada = null;
            LogNivel? nivelCapturado = null;
            string? observacaoCapturada = null;

            _servicoLogsMock
                .Setup(s => s.Enviar(It.IsAny<string>(), It.IsAny<LogContexto>(), It.IsAny<LogNivel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, LogContexto, LogNivel, string, string?>((m, c, n, o, r) =>
                {
                    mensagemCapturada = m;
                    nivelCapturado = n;
                    observacaoCapturada = o;
                })
                .Returns(Task.CompletedTask);

            // Act
            await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                _servicoLogsMock.Object,
                operacao: "AlterarPropostaRascunho",
                propostaId: 456,
                valorBancoAnterior: true,
                valorRecebidoDto: null,
                valorFinalPersistido: true,
                nomeFormacao: "Curso Rascunho",
                areaPromotoraId: 20,
                situacao: "Rascunho",
                usuario: "admin"
            );

            // Assert
            mensagemCapturada.Should().NotBeNull();
            mensagemCapturada.Should().StartWith("[INTEGRA_SGA]");
            mensagemCapturada.Should().Contain("PropostaId: 456");
            mensagemCapturada.Should().Contain("Banco: TRUE");
            mensagemCapturada.Should().Contain("DTO: NULL");
            mensagemCapturada.Should().Contain("Final: TRUE");
            mensagemCapturada.Should().Contain("PRESERVADO");
            nivelCapturado.Should().Be(LogNivel.Informacao);
            observacaoCapturada.Should().Contain("\"mudouDeTrueParaFalse\":false");
        }

        [Fact]
        public async Task DadoServicoLogsNulo_QuandoRegistrarLogSga_NaoDeveLancarExcecao()
        {
            // Act
            var act = async () => await LogIntegrarSgaHelper.RegistrarLogSgaAsync(
                null,
                operacao: "InserirProposta",
                propostaId: 789,
                valorBancoAnterior: null,
                valorRecebidoDto: true,
                valorFinalPersistido: true
            );

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
