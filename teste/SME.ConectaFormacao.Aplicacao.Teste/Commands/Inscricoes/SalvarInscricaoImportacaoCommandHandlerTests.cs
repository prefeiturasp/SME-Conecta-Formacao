using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoImportacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class SalvarInscricaoImportacaoCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarInscricaoImportacaoCommandHandler _handler;
        private readonly Faker<Inscricao> _fakerInscricao;

        public SalvarInscricaoImportacaoCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<SalvarInscricaoImportacaoCommandHandler>();

            _fakerInscricao = new Faker<Inscricao>("pt_BR")
                .RuleFor(i => i.Id, f => f.Random.Long(1))
                .RuleFor(i => i.UsuarioId, f => f.Random.Long(1))
                .RuleFor(i => i.PropostaTurmaId, f => f.Random.Long(1))
                .RuleFor(i => i.Situacao, SituacaoInscricao.AguardandoAnalise);
        }

        [Fact]
        public async Task DadoInscricaoValida_QuandoHouverVagaDisponivel_EntaoDeveInserirConfirmarEComitarTransacao()
        {
            // Arrange
            var inscricao = _fakerInscricao.Generate();
            var comando = new SalvarInscricaoImportacaoCommand(inscricao);
            var transacaoMock = new Mock<IDbTransaction>();

            _mocker.GetMock<ITransacao>()
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            Assert.Equal(SituacaoInscricao.Confirmada, inscricao.Situacao);

            _mocker.GetMock<IRepositorioInscricao>()
                .Verify(r => r.Inserir(inscricao), Times.Once);

            _mocker.GetMock<IRepositorioInscricao>()
                .Verify(r => r.Atualizar(inscricao), Times.Once); // Atualiza status para Confirmada

            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoInscricaoValida_QuandoNaoHouverVagaDisponivel_EntaoDeveLancarExcecaoNegocioERealizarRollback()
        {
            // Arrange
            var inscricao = _fakerInscricao.Generate();
            var comando = new SalvarInscricaoImportacaoCommand(inscricao);
            var transacaoMock = new Mock<IDbTransaction>();

            _mocker.GetMock<ITransacao>()
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ConfirmarInscricaoVaga(It.IsAny<Inscricao>()))
                .ReturnsAsync(false); // Simula falta de vaga

            // Act
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));

            // Assert
            Assert.Equal(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA, excecao.Message);

            _mocker.GetMock<IRepositorioInscricao>()
                .Verify(r => r.Inserir(inscricao), Times.Once);

            _mocker.GetMock<IRepositorioInscricao>()
                .Verify(r => r.Atualizar(It.IsAny<Inscricao>()), Times.Never);

            transacaoMock.Verify(t => t.Commit(), Times.Never);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoBancoDeDados_QuandoTentarInserir_EntaoDeveRelancarExcecaoERealizarRollback()
        {
            // Arrange
            var inscricao = _fakerInscricao.Generate();
            var comando = new SalvarInscricaoImportacaoCommand(inscricao);
            var transacaoMock = new Mock<IDbTransaction>();

            _mocker.GetMock<ITransacao>()
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.Inserir(It.IsAny<Inscricao>()))
                .ThrowsAsync(new Exception("Erro de conexão"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(comando, CancellationToken.None));

            transacaoMock.Verify(t => t.Commit(), Times.Never);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}