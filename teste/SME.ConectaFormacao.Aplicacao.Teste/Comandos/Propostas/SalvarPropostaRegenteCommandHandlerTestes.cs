using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class SalvarPropostaRegenteCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SalvarPropostaRegenteCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarPropostaRegenteCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _mapperMock = _mocker.GetMock<IMapper>();
            _repositorioPropostaMock = _mocker.GetMock<IRepositorioProposta>();
            _transacaoMock = _mocker.GetMock<ITransacao>();
            _dbTransactionMock = new Mock<IDbTransaction>();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _handler = _mocker.CreateInstance<SalvarPropostaRegenteCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCpfInvalido_QuandoHandle_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var command = CriarCommand(propostaId);
            command.PropostaRegenteDTO.Cpf = "11111111111";

            // Act
            var acao = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.CPF_INVALIDO);

            _transacaoMock.Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoNovoRegenteComTurmas_QuandoHandle_EntaoDeveInserirERetornarId()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var command = CriarCommand(propostaId);
            var regenteMapeado = CriarPropostaRegente();
            regenteMapeado.Id = _faker.Random.Long(1, 1000);
            regenteMapeado.Turmas = new[]
            {
                new PropostaRegenteTurma { Id = 10, TurmaId = 1, PropostaRegenteId = regenteMapeado.Id }
            };

            _mapperMock
                .Setup(m => m.Map<PropostaRegente>(command.PropostaRegenteDTO))
                .Returns(regenteMapeado);

            _repositorioPropostaMock
                .Setup(r => r.ObterPropostaRegentePorId(It.IsAny<long>()))
                .ReturnsAsync((PropostaRegente)null!);

            _repositorioPropostaMock
                .Setup(r => r.ObterRegenteTurmasPorRegenteId(It.IsAny<long>()))
                .ReturnsAsync(new List<PropostaRegenteTurma>());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _repositorioPropostaMock
                .Setup(r => r.InserirPropostaRegente(It.IsAny<long>(), It.IsAny<PropostaRegente>()))
                .Returns(Task.CompletedTask);

            _repositorioPropostaMock
                .Setup(r => r.InserirPropostaRegenteTurma(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaRegenteTurma>>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().Be(regenteMapeado.Id);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);

            _repositorioPropostaMock.Verify(r => r.InserirPropostaRegente(propostaId, regenteMapeado), Times.Once);
        }

        [Fact]
        public async Task DadoRegenteExistenteComAlteracoes_QuandoHandle_EntaoDeveAtualizarTurmas()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var regenteExistente = CriarPropostaRegente();
            var command = CriarCommand(propostaId, regenteExistente.Id);
            command.PropostaRegenteDTO.NomeRegente = _faker.Person.FullName;
            command.PropostaRegenteDTO.MiniBiografia = _faker.Lorem.Paragraph();
            command.PropostaRegenteDTO.Turmas = new[] { new PropostaRegenteTurmaDTO { TurmaId = 200, Nome = "Turma B" } };

            var regenteMapeado = CriarPropostaRegente();
            regenteMapeado.Id = regenteExistente.Id;
            regenteMapeado.PropostaId = regenteExistente.PropostaId;
            regenteMapeado.NomeRegente = command.PropostaRegenteDTO.NomeRegente;
            regenteMapeado.MiniBiografia = command.PropostaRegenteDTO.MiniBiografia;
            regenteMapeado.RegistroFuncional = regenteExistente.RegistroFuncional;
            regenteMapeado.Cpf = regenteExistente.Cpf;
            regenteMapeado.ProfissionalRedeMunicipal = regenteExistente.ProfissionalRedeMunicipal;
            regenteMapeado.Turmas = new[]
            {
                new PropostaRegenteTurma { Id = 2, TurmaId = 200, PropostaRegenteId = regenteExistente.Id }
            };

            var turmaAntes = new PropostaRegenteTurma { Id = 1, TurmaId = 100, PropostaRegenteId = regenteExistente.Id };

            _mapperMock
                .Setup(m => m.Map<PropostaRegente>(command.PropostaRegenteDTO))
                .Returns(regenteMapeado);

            _repositorioPropostaMock
                .Setup(r => r.ObterPropostaRegentePorId(regenteExistente.Id))
                .ReturnsAsync(regenteExistente);

            _repositorioPropostaMock
                .Setup(r => r.ObterRegenteTurmasPorRegenteId(regenteExistente.Id))
                .ReturnsAsync(new[] { turmaAntes });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _repositorioPropostaMock
                .Setup(r => r.AtualizarPropostaRegente(It.IsAny<PropostaRegente>()))
                .Returns(Task.CompletedTask);

            _repositorioPropostaMock
                .Setup(r => r.ExcluirPropostaRegenteTurmas(It.IsAny<IEnumerable<PropostaRegenteTurma>>()))
                .Returns(Task.CompletedTask);

            _repositorioPropostaMock
                .Setup(r => r.InserirPropostaRegenteTurma(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaRegenteTurma>>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().Be(regenteExistente.Id);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _repositorioPropostaMock.Verify(r => r.AtualizarPropostaRegente(It.Is<PropostaRegente>(p => p.Id == regenteExistente.Id)), Times.Once);
            _repositorioPropostaMock.Verify(r => r.ExcluirPropostaRegenteTurmas(It.Is<IEnumerable<PropostaRegenteTurma>>(t => t.Any(x => x.Id == turmaAntes.Id))), Times.Once);
            _repositorioPropostaMock.Verify(r => r.InserirPropostaRegenteTurma(regenteExistente.Id, It.IsAny<IEnumerable<PropostaRegenteTurma>>()), Times.Once);
        }

        private SalvarPropostaRegenteCommand CriarCommand(long propostaId, long regenteId = 0)
        {
            return new SalvarPropostaRegenteCommand(propostaId, new PropostaRegenteDTO
            {
                Id = regenteId,
                NomeRegente = _faker.Person.FullName,
                RegistroFuncional = _faker.Random.ReplaceNumbers("######"),
                Cpf = "12345678909",
                ProfissionalRedeMunicipal = true,
                MiniBiografia = _faker.Lorem.Paragraph(),
                Turmas = new[] { new PropostaRegenteTurmaDTO { TurmaId = 1, Nome = "Turma A" } }
            });
        }

        private PropostaRegente CriarPropostaRegente()
        {
            return new PropostaRegente
            {
                Id = _faker.Random.Long(1, 1000),
                PropostaId = _faker.Random.Long(1, 1000),
                NomeRegente = _faker.Person.FullName,
                RegistroFuncional = _faker.Random.ReplaceNumbers("######"),
                Cpf = "12345678901",
                ProfissionalRedeMunicipal = true,
                MiniBiografia = _faker.Lorem.Paragraph(),
                Turmas = new[]
                {
                    new PropostaRegenteTurma { Id = 1, TurmaId = 100, PropostaRegenteId = 1 }
                }
            };
        }
    }
}
