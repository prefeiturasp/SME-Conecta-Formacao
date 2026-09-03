using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoReanaliseParecerista;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Notificacoes
{
    public class GerarNotificacaoReanalisePareceristaCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly GerarNotificacaoReanalisePareceristaCommandHandler _sut;

        public GerarNotificacaoReanalisePareceristaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<GerarNotificacaoReanalisePareceristaCommandHandler>();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarHandle_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var comando = new GerarNotificacaoReanalisePareceristaCommand(
                new Proposta { Id = 2, NomeFormacao = "Reanalise Teste" },
                [new() { Login = "login2" }]
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "http://reanalise.com/{0}" });

            var usuariosMapeados = new List<NotificacaoUsuario> { new() { Login = "login2" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<NotificacaoUsuario>>(It.IsAny<IEnumerable<PropostaPareceristaResumidoDTO>>()))
                .Returns(usuariosMapeados);

            _mapperMock.Setup(m => m.Map<EnviarEmailDto>(It.IsAny<object>()))
                .Returns(new EnviarEmailDto { EmailDestinatario = "teste@teste.com" });

            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("login2"))
                .ReturnsAsync(new Usuario { Login = "login2", Email = "email2@teste.com" });

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ReturnsAsync(2);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioNotificacaoMock.Verify(r => r.Inserir(It.Is<Notificacao>(n =>
                n.Titulo == "Proposta 2 - Reanalise Teste foi atribuída a você" &&
                n.Mensagem.Contains("http://reanalise.com/2") &&
                n.Mensagem.Contains("parecer final") &&
                n.TipoEnvio == NotificacaoTipoEnvio.Email)), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}
