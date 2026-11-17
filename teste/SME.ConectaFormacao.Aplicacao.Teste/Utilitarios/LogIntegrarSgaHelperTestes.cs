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
                new LogIntegrarSgaParametros
                {
                    Operacao = "AlterarProposta",
                    PropostaId = 123,
                    ValorBancoAnterior = true,
                    ValorRecebidoDto = false,
                    ValorFinalPersistido = false,
                    NomeFormacao = "Curso Teste",
                    AreaPromotoraId = 10,
                    Situacao = "Cadastrada",
                    Usuario = "diegomoreno"
                }
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
                new LogIntegrarSgaParametros
                {
                    Operacao = "AlterarPropostaRascunho",
                    PropostaId = 456,
                    ValorBancoAnterior = true,
                    ValorRecebidoDto = null,
                    ValorFinalPersistido = true,
                    NomeFormacao = "Curso Rascunho",
                    AreaPromotoraId = 20,
                    Situacao = "Rascunho",
                    Usuario = "admin"
                }
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
                new LogIntegrarSgaParametros
                {
                    Operacao = "InserirProposta",
                    PropostaId = 789,
                    ValorBancoAnterior = null,
                    ValorRecebidoDto = true,
                    ValorFinalPersistido = true
                }
            );

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}
