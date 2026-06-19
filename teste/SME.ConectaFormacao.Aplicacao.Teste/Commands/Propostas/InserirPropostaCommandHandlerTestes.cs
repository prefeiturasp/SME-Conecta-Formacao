using AutoMapper;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class InserirPropostaCommandHandlerTestes
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IRepositorioProposta> repositorio;
        private readonly Mock<ITransacao> transacao;
        private readonly Mock<IDbTransaction> transaction;

        private readonly InserirPropostaCommandHandler handler;

        public InserirPropostaCommandHandlerTestes()
        {
            mediator = new Mock<IMediator>();
            mapper = new Mock<IMapper>();
            repositorio = new Mock<IRepositorioProposta>();
            transacao = new Mock<ITransacao>();
            transaction = new Mock<IDbTransaction>();

            transacao.Setup(x => x.Iniciar())
                .Returns(transaction.Object);

            handler = new InserirPropostaCommandHandler(
                mediator.Object,
                mapper.Object,
                transacao.Object,
                repositorio.Object);
        }

        #region Constructor

        [Fact]
        public void Deve_lancar_excecao_quando_mediator_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InserirPropostaCommandHandler(
                    null!,
                    mapper.Object,
                    transacao.Object,
                    repositorio.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_mapper_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InserirPropostaCommandHandler(
                    mediator.Object,
                    null!,
                    transacao.Object,
                    repositorio.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_transacao_for_nula()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InserirPropostaCommandHandler(
                    mediator.Object,
                    mapper.Object,
                    null!,
                    repositorio.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_repositorio_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InserirPropostaCommandHandler(
                    mediator.Object,
                    mapper.Object,
                    transacao.Object,
                    null!));
        }

        #endregion

        [Fact]
        public async Task Deve_inserir_proposta_com_sucesso()
        {
            var dto = new PropostaDTO();

            var proposta = new Proposta();

            mapper.Setup(x => x.Map<Proposta>(dto))
                .Returns(proposta);

            repositorio.Setup(x => x.Inserir(proposta))
                .ReturnsAsync(15);

            var command = new InserirPropostaCommand(20, dto);

            var retorno = await handler.Handle(command, CancellationToken.None);

            Assert.True(retorno.Sucesso);
            Assert.Equal(15, retorno.EntidadeId);

            Assert.Equal(20, proposta.AreaPromotoraId);

            mapper.Verify(x => x.Map<Proposta>(dto), Times.Once);

            repositorio.Verify(x => x.Inserir(proposta), Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<ValidarFuncaoEspecificaOutrosCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<ValidarCriterioValidacaoInscricaoOutrosCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<ValidarAreaPromotoraCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<ValidarResponsavelDfCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mediator.Verify(x =>
                    x.Send(It.IsAny<SalvarPropostaCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            transaction.Verify(x => x.Commit(), Times.Once);

            transaction.Verify(x => x.Dispose(), Times.Once);

            transaction.Verify(x => x.Rollback(), Times.Never);
        }

        [Fact]
        public async Task Deve_realizar_rollback_quando_ocorrer_erro()
        {
            var dto = new PropostaDTO();

            var proposta = new Proposta();

            mapper.Setup(x => x.Map<Proposta>(dto))
                .Returns(proposta);

            repositorio.Setup(x => x.Inserir(It.IsAny<Proposta>()))
                .ThrowsAsync(new Exception("erro"));

            var command = new InserirPropostaCommand(1, dto);

            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None));

            transaction.Verify(x => x.Rollback(), Times.Once);

            transaction.Verify(x => x.Dispose(), Times.Once);

            transaction.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Deve_enviar_salvar_proposta_com_id_correto()
        {
            var dto = new PropostaDTO();

            var proposta = new Proposta();

            mapper.Setup(x => x.Map<Proposta>(dto))
                .Returns(proposta);

            repositorio.Setup(x => x.Inserir(It.IsAny<Proposta>()))
                .ReturnsAsync(123);

            SalvarPropostaCommand? commandRecebido = null;

            mediator.Setup(x => x.Send(It.IsAny<SalvarPropostaCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((c, _) =>
                {
                    commandRecebido = (SalvarPropostaCommand)c;
                })
                .Returns(Task.FromResult(true));

            await handler.Handle(
                new InserirPropostaCommand(10, dto),
                CancellationToken.None);

            Assert.NotNull(commandRecebido);
            Assert.Equal(123, commandRecebido.PropostaId);
        }
    }
}
