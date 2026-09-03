using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class SalvarPropostaRegenteCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarPropostaRegenteCommandHandler _sut;
        private readonly Faker _faker;

        public SalvarPropostaRegenteCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<SalvarPropostaRegenteCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoRegenteComCpfInvalido_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var dto = new PropostaRegenteDTO { Id = 1, Cpf = "123" };
            var comando = new SalvarPropostaRegenteCommand(1, dto);

            var regenteMapeado = new PropostaRegente { Id = 1, Cpf = "123" };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaRegente>(dto))
                .Returns(regenteMapeado);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoNovoRegente_QuandoExecutar_EntaoDeveInserirESalvarTurmas()
        {
            // Arrange
            var dto = new PropostaRegenteDTO
            {
                Id = 0,
                Cpf = _faker.Person.Cpf(),
                Turmas = [new() { TurmaId = 1 }]
            };
            var comando = new SalvarPropostaRegenteCommand(1, dto);

            var regenteMapeado = new PropostaRegente
            {
                Id = 0,
                Cpf = dto.Cpf,
                Turmas = [new() { TurmaId = 1, Id = 0 }]
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaRegente>(dto))
                .Returns(regenteMapeado);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegenteTurmasPorRegenteId(0))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirPropostaRegente(1, regenteMapeado), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirPropostaRegenteTurma(0, It.Is<IEnumerable<PropostaRegenteTurma>>(t => t.Count() == 1)), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.AtualizarPropostaRegente(It.IsAny<PropostaRegente>()), Times.Never);
            transacaoMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoRegenteExistenteComAlteracao_QuandoExecutar_EntaoDeveAtualizar()
        {
            // Arrange
            var dto = new PropostaRegenteDTO { Id = 1, NomeRegente = "Novo Nome" };
            var comando = new SalvarPropostaRegenteCommand(1, dto);

            var regenteAntes = new PropostaRegente { Id = 1, NomeRegente = "Nome Antigo" };
            var regenteDepois = new PropostaRegente { Id = 1, NomeRegente = "Novo Nome", Turmas = [] };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterPropostaRegentePorId(1))
                .ReturnsAsync(regenteAntes);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaRegente>(dto))
                .Returns(regenteDepois);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegenteTurmasPorRegenteId(1))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.AtualizarPropostaRegente(regenteDepois), Times.Once);
            transacaoMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoBanco_QuandoExecutar_EntaoDeveFazerRollback()
        {
            // Arrange
            var dto = new PropostaRegenteDTO { Id = 0 };
            var comando = new SalvarPropostaRegenteCommand(1, dto);
            var regenteDepois = new PropostaRegente { Id = 0, Turmas = new List<PropostaRegenteTurma>() };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaRegente>(dto))
                .Returns(regenteDepois);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterRegenteTurmasPorRegenteId(0))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.InserirPropostaRegente(1, regenteDepois))
                .ThrowsAsync(new Exception("Erro BD"));

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro BD");
            transacaoMock.Verify(m => m.Rollback(), Times.Once);
            transacaoMock.Verify(m => m.Dispose(), Times.Once);
        }
    }
}
