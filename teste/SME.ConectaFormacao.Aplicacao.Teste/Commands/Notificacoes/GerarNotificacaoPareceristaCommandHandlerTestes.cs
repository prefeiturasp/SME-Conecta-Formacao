using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoParecerista;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Notificacoes
{
    public class GerarNotificacaoPareceristaCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly GerarNotificacaoPareceristaCommandHandler _sut;

        public GerarNotificacaoPareceristaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<GerarNotificacaoPareceristaCommandHandler>();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarHandle_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var comando = new GerarNotificacaoPareceristaCommand(
                new Proposta { Id = 1, NomeFormacao = "Formacao Teste" },
                [new() { Login = "login1" }]
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "http://teste.com/{0}" });

            var usuariosMapeados = new List<NotificacaoUsuario> { new() { Login = "login1" } };
            _mapperMock.Setup(m => m.Map<IEnumerable<NotificacaoUsuario>>(It.IsAny<IEnumerable<PropostaPareceristaResumidoDTO>>()))
                .Returns(usuariosMapeados);

            _mapperMock.Setup(m => m.Map<EnviarEmailDto>(It.IsAny<object>()))
                .Returns(new EnviarEmailDto { EmailDestinatario = "teste@teste.com" });

            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("login1"))
                .ReturnsAsync(new Usuario { Login = "login1", Email = "email1@teste.com" });

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ReturnsAsync(1);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioNotificacaoMock.Verify(r => r.Inserir(It.Is<Notificacao>(n =>
                n.Titulo == "A Proposta 1 - Formacao Teste foi atribuída a você" &&
                n.Mensagem.Contains("http://teste.com/1") &&
                n.TipoEnvio == NotificacaoTipoEnvio.Email)), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}
