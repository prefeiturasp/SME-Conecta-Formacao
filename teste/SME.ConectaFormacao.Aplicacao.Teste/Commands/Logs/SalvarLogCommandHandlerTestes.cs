using AutoMapper;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Logs
{
    public class SalvarLogCommandHandlerTestes
    {
        private readonly Mock<IRepositorioLog> repositorioMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ITransacao> transacaoMock;
        private readonly Mock<IDbTransaction> dbTransactionMock;

        private readonly SalvarLogCommandHandler handler;

        public SalvarLogCommandHandlerTestes()
        {
            repositorioMock = new Mock<IRepositorioLog>();
            mapperMock = new Mock<IMapper>();
            transacaoMock = new Mock<ITransacao>();
            dbTransactionMock = new Mock<IDbTransaction>();

            transacaoMock
                .Setup(x => x.Iniciar())
                .Returns(dbTransactionMock.Object);

            handler = new SalvarLogCommandHandler(
                repositorioMock.Object,
                mapperMock.Object,
                transacaoMock.Object);
        }

        [Fact]
        public async Task Deve_salvar_log_com_sucesso()
        {
            var dto = new LogDTO();
            var entidade = new Log();

            mapperMock
                .Setup(x => x.Map<Log>(dto))
                .Returns(entidade);

            repositorioMock
                .Setup(x => x.Inserir(dbTransactionMock.Object, entidade))
                .ReturnsAsync(0L);

            var command = new SalvarLogCommand(dto);

            var resultado = await handler.Handle(command, CancellationToken.None);

            Assert.True(resultado);

            mapperMock.Verify(x => x.Map<Log>(dto), Times.Once);

            repositorioMock.Verify(
                x => x.Inserir(dbTransactionMock.Object, entidade),
                Times.Once);

            dbTransactionMock.Verify(x => x.Commit(), Times.Once);
            dbTransactionMock.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task Deve_realizar_rollback_quando_ocorrer_erro()
        {
            var dto = new LogDTO();
            var entidade = new Log();

            mapperMock
                .Setup(x => x.Map<Log>(dto))
                .Returns(entidade);

            repositorioMock
                .Setup(x => x.Inserir(dbTransactionMock.Object, entidade))
                .ThrowsAsync(new Exception("Erro"));

            var command = new SalvarLogCommand(dto);

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
                    transacaoMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_mapper_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    repositorioMock.Object,
                    null!,
                    transacaoMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_transacao_for_nula()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SalvarLogCommandHandler(
                    repositorioMock.Object,
                    mapperMock.Object,
                    null!));
        }
    }
}
