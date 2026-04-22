using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Eventos.Relatorios;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Eventos
{
    public class NotificarRelatorioEmitidoEventoHandlerTestes
    {
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IRepositorioNotificacaoUsuario> _repositorioNotificacaoUsuarioMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IServicoLogs> _servicoLogsMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly NotificarRelatorioEmitidoEventoHandler _sut;

        public NotificarRelatorioEmitidoEventoHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _repositorioNotificacaoUsuarioMock = mocker.GetMock<IRepositorioNotificacaoUsuario>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _mapperMock = mocker.GetMock<IMapper>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _servicoLogsMock = mocker.GetMock<IServicoLogs>();

            _dbTransactionMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<NotificarRelatorioEmitidoEventoHandler>();
        }

        [Fact]
        public async Task DadoEventoValido_QuandoProcessarHandle_EntaoDeveSalvarNotificacaoCommitarTransacaoEPublicarNaFila()
        {
            // Arrange
            var notificacaoDto = new NotificacaoDTO
            {
                Id = 1,
                Titulo = "Relatório Concluído",
                Mensagem = "Seu relatório está pronto"
            };

            var usuariosAlvo = new List<Usuario>
            {
                new() { Login = "1234567", Nome = "Diego", Email = "diego@sme.sp.gov.br" }
            };

            var evento = new NotificarRelatorioEmitidoEvento(notificacaoDto, usuariosAlvo);
            var notificacaoSignalR = new NotificacaoSignalRDTO { Titulo = "Relatório Concluído" };

            _mapperMock.Setup(m => m.Map<NotificacaoSignalRDTO>(It.IsAny<Notificacao>()))
                       .Returns(notificacaoSignalR);

            // Act
            Func<Task> acao = async () => await _sut.Handle(evento, CancellationToken.None);

            // Assert
            await acao.Should().NotThrowAsync();

            _repositorioNotificacaoMock.Verify(r => r.Inserir(It.Is<Notificacao>(n =>
                n.Titulo == "Relatório Concluído" &&
                n.Usuarios.Count() == 1)), Times.Once);

            _repositorioNotificacaoUsuarioMock.Verify(r => r.InserirUsuarios(
                _dbTransactionMock.Object,
                It.IsAny<IEnumerable<NotificacaoUsuario>>(),
                It.IsAny<long>()), Times.Once);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == "conecta.enviar.notificacao"),
                It.IsAny<CancellationToken>()), Times.Once);

            _servicoLogsMock.Verify(s => s.Enviar(It.IsAny<string>(), It.IsAny<LogContexto>(), It.IsAny<LogNivel>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DadoExcecaoDuranteTransacaoDeBanco_QuandoProcessarHandle_EntaoDeveLogarErroCriticoNaoInterrompendoFluxo()
        {
            // Arrange
            var notificacaoDto = new NotificacaoDTO { Titulo = "Relatório Concluído", Id = 99 };
            var evento = new NotificarRelatorioEmitidoEvento(notificacaoDto, []);
            var excecaoEsperada = new Exception("Deadlock no banco de dados");

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ThrowsAsync(excecaoEsperada);

            // Act
            Func<Task> acao = async () => await _sut.Handle(evento, CancellationToken.None);

            // Assert
            await acao.Should().NotThrowAsync();

            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);

            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);

            _servicoLogsMock.Verify(s => s.Enviar(
                It.Is<string>(msg => msg.Contains("Erro a notificar relatório emitido") && msg.Contains("Deadlock")),
                LogContexto.Notificacao,
                LogNivel.Critico,
                It.Is<string>(obs => obs.Contains("Titulo=Relatório Concluído | Id=99")),
                It.IsAny<string>()), Times.Once);
        }
    }
}
