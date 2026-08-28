using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Notificacoes
{
    public class GerarNotificacaoDFCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IRepositorioNotificacaoUsuario> _repositorioNotificacaoUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly GerarNotificacaoDFCommandHandler _sut;

        public GerarNotificacaoDFCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _repositorioNotificacaoUsuarioMock = mocker.GetMock<IRepositorioNotificacaoUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<GerarNotificacaoDFCommandHandler>();
        }

        [Fact]
        public async Task DadoUsuarioNaoEncontrado_QuandoChamarHandle_EntaoLancaExcecao()
        {
            // Arrange
            var comando = new GerarNotificacaoDFCommand(
                new Proposta { Id = 1, RfResponsavelDf = "rf1" },
                new PropostaPareceristaResumidoDTO { Nome = "Parecerista" }
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "http://teste.com/{0}" });

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarHandle_EntaoDeveSalvarEPublicarNoRabbit()
        {
            // Arrange
            var comando = new GerarNotificacaoDFCommand(
                new Proposta { Id = 1, NomeFormacao = "Formacao Teste", RfResponsavelDf = "rf1" },
                new PropostaPareceristaResumidoDTO { Nome = "Parecerista", Login = "login1" }
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "http://teste.com/{0}" });

            var usuario = new Usuario { Login = "rf1", Email = "rf1@teste.com" };
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("rf1")).ReturnsAsync(usuario);

            var notificacoesUsuarios = new List<NotificacaoUsuario> { new NotificacaoUsuario { Login = "rf1" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<NotificacaoUsuario>>(It.IsAny<IEnumerable<Usuario>>()))
                .Returns(notificacoesUsuarios);

            var signalRDto = new NotificacaoSignalRDTO { Titulo = "Titulo Teste" };
            _mapperMock.Setup(m => m.Map<NotificacaoSignalRDTO>(It.IsAny<Notificacao>()))
                .Returns(signalRDto);

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ReturnsAsync(1);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioNotificacaoMock.Verify(r => r.Inserir(It.Is<Notificacao>(n =>
                n.Titulo == "Proposta 1 - Formacao Teste foi analisada pelo Parecerista" &&
                n.TipoEnvio == NotificacaoTipoEnvio.SignalR)), Times.Once);

            _repositorioNotificacaoUsuarioMock.Verify(r => r.InserirUsuarios(_dbTransactionMock.Object, notificacoesUsuarios, 1), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == "conecta.enviar.notificacao" && c.Filtros == signalRDto), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
