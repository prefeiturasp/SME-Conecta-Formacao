using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Notificacoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Notificacoes
{
    public class GerarNotificacaoCommandHandlerBaseTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IRepositorioNotificacaoUsuario> _repositorioNotificacaoUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly TestableGerarNotificacaoCommandHandlerBase _sut;

        public GerarNotificacaoCommandHandlerBaseTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _repositorioNotificacaoUsuarioMock = mocker.GetMock<IRepositorioNotificacaoUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<TestableGerarNotificacaoCommandHandlerBase>();
        }

        [Fact]
        public async Task DadoNotificacaoValida_QuandoChamarProcessarNotificacaoAsync_EntaoDeveInserirEPublicarNaFilaRabbit()
        {
            // Arrange
            var notificacao = new Notificacao
            {
                Usuarios =
                [
                    new() { Email = "teste1@teste.com" },
                    new() { Email = "teste1@teste.com" }, // Duplicado para testar filtro
                    new() { Email = "teste2@teste.com" }
                ],
                Titulo = "Titulo Teste",
                Mensagem = "Mensagem Teste"
            };

            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<EnviarEmailDto>(It.IsAny<object>()))
                .Returns(new EnviarEmailDto { EmailDestinatario = "teste@teste.com" });

            // Act
            var resultado = await _sut.ExporProcessarNotificacaoAsync(notificacao, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioNotificacaoMock.Verify(r => r.Inserir(notificacao), Times.Once);
            _repositorioNotificacaoUsuarioMock.Verify(r => r.InserirUsuarios(_dbTransactionMock.Object, notificacao.Usuarios, 1), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DadoErroAoInserir_QuandoChamarProcessarNotificacaoAsync_EntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var notificacao = new Notificacao { Usuarios = new List<NotificacaoUsuario>() };
            _repositorioNotificacaoMock.Setup(r => r.Inserir(It.IsAny<Notificacao>())).ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var act = async () => await _sut.ExporProcessarNotificacaoAsync(notificacao, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro de banco");
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
        }
    }

    public class TestableGerarNotificacaoCommandHandlerBase(
        ITransacao transacao,
        IRepositorioNotificacao repositorioNotificacao,
        IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,
        IMediator mediator,
        IMapper mapper) : 
        GerarNotificacaoCommandHandlerBase(transacao, repositorioNotificacao, repositorioNotificacaoUsuario, mediator, mapper)
    {
        public Task<bool> ExporProcessarNotificacaoAsync(Notificacao notificacao, CancellationToken cancellationToken)
        {
            return ProcessarNotificacaoAsync(notificacao, cancellationToken);
        }
    }
}
