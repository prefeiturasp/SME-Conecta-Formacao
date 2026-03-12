using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Aplicacao.Dtos.Relatorios;
using SME.ConectaFormacao.Aplicacao.Eventos.Relatorios;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioInscritosPorFormacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IGeradorRelatorioInscritosExcelService> _geradorRelatorioMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IRepositorioRelatorios> _repositorioRelatoriosMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IServicoLogs> _servicoLogsMock;
        private readonly CasoDeUsoGerarRelatorioInscritosPorFormacao _sut;

        public CasoDeUsoGerarRelatorioInscritosPorFormacaoTestes()
        {
            var mocker = new AutoMocker();

            _mediatorMock = mocker.GetMock<IMediator>();
            _geradorRelatorioMock = mocker.GetMock<IGeradorRelatorioInscritosExcelService>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _repositorioRelatoriosMock = mocker.GetMock<IRepositorioRelatorios>();
            _timeProviderMock = mocker.GetMock<TimeProvider>();
            _servicoLogsMock = mocker.GetMock<IServicoLogs>();

            _timeProviderMock.Setup(t => t.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero));

            _sut = mocker.CreateInstance<CasoDeUsoGerarRelatorioInscritosPorFormacao>();
        }

        [Fact]
        public async Task DadoFiltrosInvalidos_QuandoExecutar_EntaoDeveLogarErroDeNegocioERetornarFalse()
        {
            // Arrange
            var filtroInvalido = new FiltroRelatorioInscritosPorFormacaoDto(); 
            var mensagemRabbit = CriarMensagemRabbit(filtroInvalido);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeFalse();

            _servicoLogsMock.Verify(s => s.Enviar(
                It.Is<string>(msg => msg.Contains("Validação dos filtros para geração do relatório de inscritos por formação falhou")),
                LogContexto.Relatorio,
                LogNivel.Negocio,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);

            _repositorioRelatoriosMock.Verify(r => r.ObterDadosRelatorioInscritosPorFormacaoAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()), Times.Never);
        }

        [Fact]
        public async Task DadoDadosValidosEProcessamentoComSucesso_QuandoExecutar_EntaoDeveGerarExcelENotificarUsuario()
        {
            // Arrange
            var filtroValido = new FiltroRelatorioInscritosPorFormacaoDto { PropostaId = 123 };
            var mensagemRabbit = CriarMensagemRabbit(filtroValido);
            var usuarioDb = new Usuario { Id = 1, Nome = "Diego Ferreira Moreno", Login = "1120641" };

            var dadosBanco = new List<InscritoFormacaoQueryModel>
            {
                new() { CodigoFormacao = "245", NomeCursista = "Aluno Teste", RfCpf = "1234567" }
            };

            var urlEsperada = "https://minio.sme.sp.gov.br/relatorios/123.xlsx";

            _repositorioUsuarioMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(usuarioDb);
            _repositorioRelatoriosMock.Setup(r => r.ObterDadosRelatorioInscritosPorFormacaoAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                                      .ReturnsAsync(dadosBanco);
            _geradorRelatorioMock.Setup(g => g.GerarEArmazenarRelatorioAsync(It.IsAny<RelatorioInscritosFormacaoDto>()))
                                 .ReturnsAsync(urlEsperada);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();

            _geradorRelatorioMock.Verify(g => g.GerarEArmazenarRelatorioAsync(
                It.Is<RelatorioInscritosFormacaoDto>(dto =>
                    dto.Inscritos.Count() == 1 &&
                    dto.NomeUsuario == "LEITE CARRETA")), Times.Once);

            _mediatorMock.Verify(m => m.Publish(
                It.Is<NotificarRelatorioEmitidoEvento>(evento =>
                    evento.Notificacao.Mensagem.Contains(urlEsperada) &&
                    evento.UsuariosAlvo[0].Id == usuarioDb.Id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoProcessamento_QuandoExecutar_EntaoDeveLogarErroCriticoERetornarFalse()
        {
            // Arrange
            var filtroValido = new FiltroRelatorioInscritosPorFormacaoDto { PropostaId = 123 };
            var mensagemRabbit = CriarMensagemRabbit(filtroValido);

            _repositorioUsuarioMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(new Usuario());

            var excecaoEsperada = new Exception("Falha de conexão com o banco");
            _repositorioRelatoriosMock.Setup(r => r.ObterDadosRelatorioInscritosPorFormacaoAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                                      .ThrowsAsync(excecaoEsperada);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeFalse();

            _servicoLogsMock.Verify(s => s.Enviar(
                excecaoEsperada,
                "Erro ao gerar relatório de inscritos por formação",
                LogContexto.Relatorio,
                LogNivel.Critico,
                It.IsAny<string>()), Times.Once);

            _mediatorMock.Verify(m => m.Publish(It.IsAny<NotificarRelatorioEmitidoEvento>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private static MensagemRabbit CriarMensagemRabbit(FiltroRelatorioInscritosPorFormacaoDto filtro)
        {
            var solicitacaoDto = new SolicitacaoRelatorioInscritosPorFormacaoMensagem(
                Guid.NewGuid(),
                new UsuarioContextoDto(1, "LEITE CARRETA", "8430055"),
                DateTime.Now,
                filtro
            );

            var mensagemJson = JsonSerializer.Serialize(solicitacaoDto);

            return new MensagemRabbit
            {
                Mensagem = mensagemJson
            };
        }
    }
}
