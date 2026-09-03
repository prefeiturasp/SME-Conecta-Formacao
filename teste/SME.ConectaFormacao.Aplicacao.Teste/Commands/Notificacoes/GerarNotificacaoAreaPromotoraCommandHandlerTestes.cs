using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes.GerarNotificacaoAreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Notificacoes
{
    public class GerarNotificacaoAreaPromotoraCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioAreaPromotora> _repositorioAreaPromotoraMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly GerarNotificacaoAreaPromotoraCommandHandler _sut;

        public GerarNotificacaoAreaPromotoraCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioAreaPromotoraMock = mocker.GetMock<IRepositorioAreaPromotora>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<GerarNotificacaoAreaPromotoraCommandHandler>();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoChamarHandle_EntaoDeveRetornarVerdadeiro()
        {
            // Arrange
            var comando = new GerarNotificacaoAreaPromotoraCommand(
                new Proposta { Id = 3, NomeFormacao = "Formacao Promotora", CriadoLogin = "criador1" }
            );

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "http://promotora.com/{0}" });

            _repositorioAreaPromotoraMock.Setup(r => r.ObterAreaPromotoraPorPropostaId(3))
                .ReturnsAsync(new AreaPromotora { Nome = "Area Promotora Teste", Email = "area@teste.com" });

            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("criador1"))
                .ReturnsAsync(new Usuario { Login = "criador1", Nome = "Criador Teste", Email = "criador@teste.com" });

            _mapperMock.Setup(m => m.Map<EnviarEmailDto>(It.IsAny<object>()))
                .Returns(new EnviarEmailDto { EmailDestinatario = "teste@teste.com" });

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ReturnsAsync(3);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioNotificacaoMock.Verify(r => r.Inserir(It.Is<Notificacao>(n =>
                n.Titulo == "A Proposta 3 - Formacao Promotora foi analisada pela Comissão de Análise" &&
                n.Mensagem.Contains("http://promotora.com/3") &&
                n.TipoEnvio == NotificacaoTipoEnvio.Email &&
                n.Usuarios != null)), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}
