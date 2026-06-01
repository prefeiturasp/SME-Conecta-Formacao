using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Aplicacao.Dtos.Relatorios;
using SME.ConectaFormacao.Aplicacao.Eventos.Relatorios;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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

            NotificarRelatorioEmitidoEvento? eventoPublicacao = null;
            _mediatorMock.Setup(m => m.Publish(It.IsAny<NotificarRelatorioEmitidoEvento>(), It.IsAny<CancellationToken>()))
                         .Callback<NotificarRelatorioEmitidoEvento, CancellationToken>((evt, ct) => eventoPublicacao = evt)
                         .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();

            _geradorRelatorioMock.Verify(g => g.GerarEArmazenarRelatorioAsync(
                It.Is<RelatorioInscritosFormacaoDto>(dto =>
                    dto.Inscritos.Count() == 1 &&
                    dto.NomeUsuario == "LEITE CARRETA")), Times.Once);

            eventoPublicacao.Should().NotBeNull();
            eventoPublicacao!.Notificacao.Mensagem.Should().Contain(urlEsperada);
            eventoPublicacao.UsuariosAlvo.Should().NotBeNull();
            eventoPublicacao.UsuariosAlvo[0].Id.Should().Be(usuarioDb.Id);
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

        [Fact]
        public async Task DadoInscritosComDatasEPcd_QuandoExecutar_EntaoCamposPeriodoEPcdDevemSerFormatadosCorretamente()
        {
            // Arrange
            var filtroValido = new FiltroRelatorioInscritosPorFormacaoDto { PropostaId = 999 };
            var mensagemRabbit = CriarMensagemRabbit(filtroValido);
            var usuarioDb = new Usuario { Id = 2, Nome = "Usuário Teste", Login = "998877" };

            var modelComPcd = new InscritoFormacaoQueryModel
            {
                CodigoFormacao = "1",
                NomeCursista = "Cursista PCD",
                RfCpf = "11122233344",
                DataRealizacaoInicio = new DateTime(2025, 2, 1),
                DataRealizacaoFim = new DateTime(2025, 2, 5),
                Pcd = true,
                NecessitaAdaptacao = true,
                DescricaoAdaptacao = "Leitor de tela"
            };

            var modelSemPcd = new InscritoFormacaoQueryModel
            {
                CodigoFormacao = "2",
                NomeCursista = "Cursista Sem PCD",
                RfCpf = "55566677788",
                // sem datas e pcd nulo -> deve retornar "N/A" e periodo "N/A"
                Pcd = null
            };

            _repositorioUsuarioMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(usuarioDb);
            _repositorioRelatoriosMock.Setup(r => r.ObterDadosRelatorioInscritosPorFormacaoAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                                      .ReturnsAsync(new List<InscritoFormacaoQueryModel> { modelComPcd, modelSemPcd });

            RelatorioInscritosFormacaoDto? dtoRecebido = null;
            _geradorRelatorioMock.Setup(g => g.GerarEArmazenarRelatorioAsync(It.IsAny<RelatorioInscritosFormacaoDto>()))
                                 .Callback<RelatorioInscritosFormacaoDto>(d => dtoRecebido = d)
                                 .ReturnsAsync("https://url/relatorio.xlsx");

            _mediatorMock.Setup(m => m.Publish(It.IsAny<NotificarRelatorioEmitidoEvento>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();
            dtoRecebido.Should().NotBeNull();

            var inscritos = dtoRecebido!.Inscritos.ToList();
            inscritos.Count.Should().Be(2);

            // Verifica periodo formatado para o primeiro
            inscritos[0].Periodo.Should().Be("01/02/2025 À 05/02/2025");
            inscritos[0].Pcd.Should().Be("Sim");
            inscritos[0].PrecisaAdaptacao.Should().Be("Sim");
            inscritos[0].QualAdaptacao.Should().Be("Leitor de tela");

            // Segundo registro sem datas e sem PCD
            inscritos[1].Periodo.Should().Be("N/A");
            inscritos[1].Pcd.Should().Be("N/A");
            // quando Pcd é null, PrecisaAdaptacao deve ser string vazia por lógica do mapeamento
            inscritos[1].PrecisaAdaptacao.Should().Be("");
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoNotificacaoDeveConterDataExpiracaoCom24Horas()
        {
            // Arrange
            var filtroValido = new FiltroRelatorioInscritosPorFormacaoDto { PropostaId = 777 };
            var mensagemRabbit = CriarMensagemRabbit(filtroValido);
            var usuarioDb = new Usuario { Id = 3, Nome = "Outro Usuario", Login = "445566" };

            var dadosBanco = new List<InscritoFormacaoQueryModel>
            {
                new() { CodigoFormacao = "10", NomeCursista = "Aluno", RfCpf = "00011122233" }
            };

            var urlEsperada = "https://minio.sme.sp.gov.br/relatorios/777.xlsx";

            _repositorioUsuarioMock.Setup(r => r.ObterPorId(It.IsAny<long>())).ReturnsAsync(usuarioDb);
            _repositorioRelatoriosMock.Setup(r => r.ObterDadosRelatorioInscritosPorFormacaoAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                                      .ReturnsAsync(dadosBanco);
            _geradorRelatorioMock.Setup(g => g.GerarEArmazenarRelatorioAsync(It.IsAny<RelatorioInscritosFormacaoDto>()))
                                 .ReturnsAsync(urlEsperada);

            NotificarRelatorioEmitidoEvento? eventoCapturado = null;
            _mediatorMock.Setup(m => m.Publish(It.IsAny<NotificarRelatorioEmitidoEvento>(), It.IsAny<CancellationToken>()))
                         .Callback<NotificarRelatorioEmitidoEvento, CancellationToken>((evt, ct) => eventoCapturado = evt)
                         .Returns(Task.CompletedTask);

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();

            eventoCapturado.Should().NotBeNull();
            var dataEsperada = _timeProviderMock.Object.GetUtcNow().AddHours(24);
            eventoCapturado!.Notificacao.DataExpiracao.Should().Be(dataEsperada);
            eventoCapturado.Notificacao.Titulo.Should().Be("Relatório de inscritos por formação (.xlsx)");
            eventoCapturado.Notificacao.Categoria.Should().Be(NotificacaoCategoria.Informe);
            eventoCapturado.Notificacao.Tipo.Should().Be(NotificacaoTipo.Relatorio);
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
