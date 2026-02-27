using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class RemoverPropostaCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacao;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly RemoverPropostaCommandHandler _sut;

        public RemoverPropostaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _transacao = mocker.GetMock<ITransacao>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();

            _sut = mocker.CreateInstance<RemoverPropostaCommandHandler>();
        }

        [Fact]
        public void DadoTransacaoNula_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            ITransacao transacaoNula = null!;

            // Act
            var act = () => new RemoverPropostaCommandHandler(transacaoNula, _repositorioProposta.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("transacao");
        }

        [Fact]
        public void DadoRepositorioPropostaNulo_QuandoInstanciarHandler_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            IRepositorioProposta repositorioNulo = null!;

            // Act
            var act = () => new RemoverPropostaCommandHandler(_transacao.Object, repositorioNulo);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("repositorioProposta");
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoProcessarComando_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new RemoverPropostaCommand(1);

            _repositorioProposta
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync((Proposta)null!);

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaSemListasRelacionadas_QuandoProcessarComando_EntaoDeveRemoverApenasPropostaEMovimentacaoECommitar()
        {
            // Arrange
            var comando = new RemoverPropostaCommand(1);
            var proposta = new Proposta { Id = 1 };
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(proposta);
            ConfigurarRetornoDasListas(comando.Id, comItens: false);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _repositorioProposta.Verify(r => r.RemoverDres(It.IsAny<IEnumerable<PropostaDre>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverPublicosAlvo(It.IsAny<IEnumerable<PropostaPublicoAlvo>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverFuncoesEspecificas(It.IsAny<IEnumerable<PropostaFuncaoEspecifica>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverCriteriosValidacaoInscricao(It.IsAny<IEnumerable<PropostaCriterioValidacaoInscricao>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverVagasRemanecentes(It.IsAny<IEnumerable<PropostaVagaRemanecente>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverEncontros(It.IsAny<IEnumerable<PropostaEncontro>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverPalavrasChaves(It.IsAny<IEnumerable<PropostaPalavraChave>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverTurmas(It.IsAny<IEnumerable<PropostaTurma>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverModalidades(It.IsAny<IEnumerable<PropostaModalidade>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverAnosTurmas(It.IsAny<IEnumerable<PropostaAnoTurma>>()), Times.Never);
            _repositorioProposta.Verify(r => r.RemoverComponentesCurriculares(It.IsAny<IEnumerable<PropostaComponenteCurricular>>()), Times.Never);

            _repositorioProposta.Verify(r => r.RemoverPropostaMovimentacao(proposta.Id), Times.Once);
            _repositorioProposta.Verify(r => r.Remover(proposta), Times.Once);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComListasRelacionadas_QuandoProcessarComando_EntaoDeveRemoverTodasAsListasPropostaMovimentacaoECommitar()
        {
            // Arrange
            var comando = new RemoverPropostaCommand(1);
            var proposta = new Proposta { Id = 1 };
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(proposta);
            ConfigurarRetornoDasListas(comando.Id, comItens: true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _repositorioProposta.Verify(r => r.RemoverDres(It.Is<IEnumerable<PropostaDre>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverPublicosAlvo(It.Is<IEnumerable<PropostaPublicoAlvo>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverFuncoesEspecificas(It.Is<IEnumerable<PropostaFuncaoEspecifica>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverCriteriosValidacaoInscricao(It.Is<IEnumerable<PropostaCriterioValidacaoInscricao>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverVagasRemanecentes(It.Is<IEnumerable<PropostaVagaRemanecente>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverEncontros(It.Is<IEnumerable<PropostaEncontro>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverPalavrasChaves(It.Is<IEnumerable<PropostaPalavraChave>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverTurmas(It.Is<IEnumerable<PropostaTurma>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverModalidades(It.Is<IEnumerable<PropostaModalidade>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverAnosTurmas(It.Is<IEnumerable<PropostaAnoTurma>>(x => x.Any())), Times.Once);
            _repositorioProposta.Verify(r => r.RemoverComponentesCurriculares(It.Is<IEnumerable<PropostaComponenteCurricular>>(x => x.Any())), Times.Once);

            _repositorioProposta.Verify(r => r.RemoverPropostaMovimentacao(proposta.Id), Times.Once);
            _repositorioProposta.Verify(r => r.Remover(proposta), Times.Once);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoErroDuranteARemocao_QuandoProcessarComando_EntaoDeveRealizarRollbackEPropagarExcecao()
        {
            // Arrange
            var comando = new RemoverPropostaCommand(1);
            var proposta = new Proposta { Id = 1 };
            var transacaoDbMock = ConfigurarTransacaoComSucesso();

            _repositorioProposta.Setup(r => r.ObterPorId(comando.Id)).ReturnsAsync(proposta);
            ConfigurarRetornoDasListas(comando.Id, comItens: false);

            _repositorioProposta
                .Setup(r => r.Remover(It.IsAny<Proposta>()))
                .ThrowsAsync(new Exception("Erro simulado no banco de dados"));

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro simulado no banco de dados");

            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }

        #region Factory Methods

        private Mock<IDbTransaction> ConfigurarTransacaoComSucesso()
        {
            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacao.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);
            return transacaoDbMock;
        }

        private void ConfigurarRetornoDasListas(long propostaId, bool comItens)
        {
            _repositorioProposta.Setup(r => r.ObterDrePorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaDre>());

            _repositorioProposta.Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaPublicoAlvo>());

            _repositorioProposta.Setup(r => r.ObterFuncoesEspecificasPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaFuncaoEspecifica>());

            _repositorioProposta.Setup(r => r.ObterCriteriosValidacaoInscricaoPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaCriterioValidacaoInscricao>());

            _repositorioProposta.Setup(r => r.ObterVagasRemacenentesPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaVagaRemanecente>());

            _repositorioProposta.Setup(r => r.ObterEncontrosPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaEncontro>());

            _repositorioProposta.Setup(r => r.ObterPalavrasChavesPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaPalavraChave>());

            _repositorioProposta.Setup(r => r.ObterTurmasPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaTurma>());

            _repositorioProposta.Setup(r => r.ObterModalidadesPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaModalidade>());

            _repositorioProposta.Setup(r => r.ObterAnosTurmasPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaAnoTurma>());

            _repositorioProposta.Setup(r => r.ObterComponentesCurricularesPorId(propostaId))
                .ReturnsAsync(comItens ? [new()] : Enumerable.Empty<PropostaComponenteCurricular>());
        }

        #endregion
    }
}
