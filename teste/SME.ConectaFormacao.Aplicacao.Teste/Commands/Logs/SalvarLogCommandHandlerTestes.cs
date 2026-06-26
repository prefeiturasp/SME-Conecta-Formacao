using AutoMapper;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Logs
{
    public class SalvarLogCommandHandlerTestes
    {
        private readonly Mock<IRepositorioLog> repositorioMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ITransacao> transacaoMock;
        private readonly Mock<IMediator> mediatorMock;
        private readonly Mock<IDbTransaction> dbTransactionMock;

        private readonly SalvarLogCommandHandler handler;

        public SalvarLogCommandHandlerTestes()
        {
            repositorioMock = new Mock<IRepositorioLog>();
            mapperMock = new Mock<IMapper>();
            transacaoMock = new Mock<ITransacao>();
            mediatorMock = new Mock<IMediator>();
            dbTransactionMock = new Mock<IDbTransaction>();

            transacaoMock
                .Setup(x => x.Iniciar())
                .Returns(dbTransactionMock.Object);

            handler = new SalvarLogCommandHandler(
                repositorioMock.Object,
                mapperMock.Object,
                transacaoMock.Object,
                mediatorMock.Object);
        }

        [Fact]
        public async Task Deve_salvar_log_quando_usuario_logado_existir()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 10,
                Login = "mchiesa"
            };

            var log = new Log();

            var command = new SalvarLogCommand(
                "Entidade",
                LogNivel.Informacao,
                "Mensagem",
                "Complemento");

            mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            mapperMock
                .Setup(x => x.Map<Log>(command))
                .Returns(log);

            repositorioMock
                .Setup(x => x.Inserir(dbTransactionMock.Object, log))
                .ReturnsAsync(0L);

            // Act
            var resultado = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            Assert.Equal("10", log.CriadoPor);
            Assert.Equal("mchiesa", log.CriadoLogin);
            Assert.NotEqual(default, log.CriadoEm);

            dbTransactionMock.Verify(x => x.Commit(), Times.Once);
            dbTransactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task Deve_utilizar_usuario_sistema_quando_usuario_logado_for_nulo()
        {
            // Arrange
            var log = new Log();

            var command = new SalvarLogCommand(
                "Entidade",
                LogNivel.Informacao,
                "Mensagem",
                null);

            mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null);

            mapperMock
                .Setup(x => x.Map<Log>(command))
                .Returns(log);

            repositorioMock
                .Setup(x => x.Inserir(dbTransactionMock.Object, log))
                .ReturnsAsync(0L);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("1", log.CriadoPor);
            Assert.Equal("Sistema", log.CriadoLogin);

            dbTransactionMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Deve_realizar_rollback_quando_ocorrer_excecao()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 10,
                Login = "mchiesa"
            };

            var log = new Log();

            var command = new SalvarLogCommand(
                "Entidade",
                LogNivel.Critico,
                "Mensagem",
                null);

            mediatorMock
                .Setup(x => x.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            mapperMock
                .Setup(x => x.Map<Log>(command))
                .Returns(log);

            repositorioMock
                .Setup(x => x.Inserir(dbTransactionMock.Object, log))
                .ThrowsAsync(new Exception("Erro"));

            // Act / Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None));

            dbTransactionMock.Verify(x => x.Rollback(), Times.Once);
            dbTransactionMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public void Deve_lancar_excecao_quando_repositorio_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    null!,
                    mapperMock.Object,
                    transacaoMock.Object,
                    mediatorMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_mapper_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    repositorioMock.Object,
                    null!,
                    transacaoMock.Object,
                    mediatorMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_transacao_for_nula()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    repositorioMock.Object,
                    mapperMock.Object,
                    null!,
                    mediatorMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_mediator_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    repositorioMock.Object,
                    mapperMock.Object,
                    transacaoMock.Object,
                    null!));
        }
    }
}
