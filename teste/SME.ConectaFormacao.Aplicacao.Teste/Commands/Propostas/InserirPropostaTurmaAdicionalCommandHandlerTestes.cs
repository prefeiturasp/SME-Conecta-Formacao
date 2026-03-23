using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class InserirPropostaTurmaAdicionalCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacao;
        private readonly Mock<IRepositorioProposta> _repositorioProposta;
        private readonly Mock<IRepositorioPropostaEncontro> _repositorioPropostaEncontro;
        private readonly InserirPropostaTurmaAdicionalCommandHandler _sut;

        public InserirPropostaTurmaAdicionalCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _transacao = mocker.GetMock<ITransacao>();
            _repositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _repositorioPropostaEncontro = mocker.GetMock<IRepositorioPropostaEncontro>();

            _sut = mocker.CreateInstance<InserirPropostaTurmaAdicionalCommandHandler>();
        }

        [Fact]
        public async Task DadoTurmaSemRelacionamentosENomeSimples_QuandoProcessarComando_EntaoDeveCriarTurmaComSufixoParte2ESalvarSemFilhos()
        {
            // Arrange
            var comando = new InserirPropostaTurmaAdicionalCommand(1, 30);
            var transacaoDbMock = ConfigurarTransacaoComSucesso();
            var turmaOrigem = CriarTurmaComNomeECriador("Turma Inicial", "Usuario");

            ConfigurarRetornosDoRepositorio(turmaOrigem, [], [], [], []);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().Be(turmaOrigem.Id);

            _repositorioProposta.Verify(r => r.InserirTurma(It.Is<PropostaTurma>(t => t.Nome == "Turma Inicial - Parte 2")), Times.Once);
            _repositorioProposta.Verify(r => r.InserirPropostaTurmasDres(It.IsAny<IEnumerable<PropostaTurmaDre>>()), Times.Never);
            _repositorioPropostaEncontro.Verify(r => r.InserirEncontroTurmasAsync(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaEncontroTurma>>()), Times.Never);
            _repositorioProposta.Verify(r => r.InserirPropostaRegenteTurma(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaRegenteTurma>>()), Times.Never);
            _repositorioProposta.Verify(r => r.InserirPropostaTutorTurma(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaTutorTurma>>()), Times.Never);

            _repositorioProposta.Verify(r => r.InserirPropostaTurmaVagas(It.Is<PropostaTurmaVaga>(v => v.PropostaTurmaId == turmaOrigem.Id), 30), Times.Once);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoTurmaCriadaPorSistemaEComSufixoParte2_QuandoProcessarComando_EntaoDeveCriarTurmaComSufixoParte3()
        {
            // Arrange
            var comando = new InserirPropostaTurmaAdicionalCommand(1, 30);
            var transacaoDbMock = ConfigurarTransacaoComSucesso();
            var turmaOrigem = CriarTurmaComNomeECriador("Turma XPTO - Parte 2", "Sistema");

            ConfigurarRetornosDoRepositorio(turmaOrigem, [], [], [], []);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.InserirTurma(It.Is<PropostaTurma>(t => t.Nome == "Turma XPTO - Parte 3")), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoTurmaComTodosRelacionamentos_QuandoProcessarComando_EntaoDeveInserirTodasAsRelacoesECommitar()
        {
            // Arrange
            var comando = new InserirPropostaTurmaAdicionalCommand(1, 30);
            var transacaoDbMock = ConfigurarTransacaoComSucesso();
            var turmaOrigem = CriarTurmaComNomeECriador("Turma Completa", "Sistema");

            var dres = CriarListaDres();
            var encontros = CriarListaEncontros();
            var regentes = CriarListaRegentes();
            var tutores = CriarListaTutores();

            ConfigurarRetornosDoRepositorio(turmaOrigem, dres, encontros, regentes, tutores);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _repositorioProposta.Verify(r => r.InserirPropostaTurmasDres(dres), Times.Once);
            _repositorioPropostaEncontro.Verify(r => r.InserirEncontroTurmasAsync(encontros[0].Id, It.IsAny<IEnumerable<PropostaEncontroTurma>>()), Times.Once);
            _repositorioProposta.Verify(r => r.InserirPropostaRegenteTurma(regentes[0].Id, It.IsAny<IEnumerable<PropostaRegenteTurma>>()), Times.Once);
            _repositorioProposta.Verify(r => r.InserirPropostaTutorTurma(tutores[0].Id, It.IsAny<IEnumerable<PropostaTutorTurma>>()), Times.Once);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroDuranteAInsercao_QuandoProcessarComando_EntaoDeveRealizarRollbackEPropagarExcecao()
        {
            // Arrange
            var comando = new InserirPropostaTurmaAdicionalCommand(1, 30);
            var transacaoDbMock = ConfigurarTransacaoComSucesso();
            var turmaOrigem = CriarTurmaComNomeECriador("Turma Com Erro", "Usuario");

            ConfigurarRetornosDoRepositorio(turmaOrigem, [], [], [], []);

            _repositorioProposta
                .Setup(r => r.InserirTurma(It.IsAny<PropostaTurma>()))
                .ThrowsAsync(new Exception("Falha de banco de dados"));

            // Act
            var act = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Falha de banco de dados");

            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Dispose(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }

        #region Factory Methods

        private Mock<IDbTransaction> ConfigurarTransacaoComSucesso()
        {
            var transacaoDbMock = new Mock<IDbTransaction>();

            _transacao
                .Setup(t => t.Iniciar())
                .Returns(transacaoDbMock.Object);

            return transacaoDbMock;
        }

        private void ConfigurarRetornosDoRepositorio(
            PropostaTurma turma,
            IEnumerable<PropostaTurmaDre> dres,
            IEnumerable<PropostaEncontro> encontros,
            IEnumerable<PropostaRegente> regentes,
            IEnumerable<PropostaTutor> tutores)
        {
            _repositorioProposta.Setup(r => r.ObterTurmaPorId(It.IsAny<long>())).ReturnsAsync(turma);
            _repositorioProposta.Setup(r => r.ObterPropostaTurmasDresPorPropostaTurmaId(It.IsAny<long>())).ReturnsAsync(dres);
            _repositorioPropostaEncontro.Setup(r => r.ObterEncontrosPorPropostaTurmaAsync(It.IsAny<long>())).ReturnsAsync(encontros);
            _repositorioProposta.Setup(r => r.ObterRegentesPorPropostaTurmaId(It.IsAny<long>())).ReturnsAsync(regentes);
            _repositorioProposta.Setup(r => r.ObterTutoresPorPropostaTurmaId(It.IsAny<long>())).ReturnsAsync(tutores);
        }

        private static PropostaTurma CriarTurmaComNomeECriador(string nome, string criadoPor)
        {
            return new PropostaTurma
            {
                Id = 99,
                Nome = nome,
                CriadoPor = criadoPor
            };
        }

        private static List<PropostaTurmaDre> CriarListaDres()
        {
            return [new PropostaTurmaDre { Id = 1, DreId = 10 }];
        }

        private static List<PropostaEncontro> CriarListaEncontros()
        {
            return [new PropostaEncontro { Id = 1, Local = "Local 1" }];
        }

        private static List<PropostaRegente> CriarListaRegentes()
        {
            return [new PropostaRegente { Id = 1, NomeRegente = "Regente 1" }];
        }

        private static List<PropostaTutor> CriarListaTutores()
        {
            return [new PropostaTutor { Id = 1, NomeTutor = "Tutor 1" }];
        }

        #endregion
    }
}
