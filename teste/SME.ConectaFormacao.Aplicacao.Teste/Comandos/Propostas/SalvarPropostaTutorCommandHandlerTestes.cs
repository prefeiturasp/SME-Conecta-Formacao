using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using System.Data;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class SalvarPropostaTutorCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SalvarPropostaTutorCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarPropostaTutorCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _mapperMock = _mocker.GetMock<IMapper>();
            _repositorioPropostaMock = _mocker.GetMock<IRepositorioProposta>();
            _transacaoMock = _mocker.GetMock<ITransacao>();
            _dbTransactionMock = new Mock<IDbTransaction>();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _handler = _mocker.CreateInstance<SalvarPropostaTutorCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCpfInvalido_QuandoHandle_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var command = CriarCommand(propostaId);
            command.PropostaTutorDto.Cpf = "11111111111";

            // Act
            var acao = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.CPF_INVALIDO);

            _transacaoMock.Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoNovoTutorComTurmas_QuandoHandle_EntaoDeveInserirERetornarId()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var command = CriarCommand(propostaId);
            var tutorMapeado = CriarPropostaTutor();
            tutorMapeado.Id = _faker.Random.Long(1, 1000);
            tutorMapeado.Turmas = new[]
            {
                new PropostaTutorTurma { Id = 10, TurmaId = 1, PropostaTutorId = tutorMapeado.Id }
            };

            _mapperMock
                .Setup(m => m.Map<PropostaTutor>(command.PropostaTutorDto))
                .Returns(tutorMapeado);

            _repositorioPropostaMock
                .Setup(r => r.ObterPropostaTutorPorId(It.IsAny<long>()))
                .ReturnsAsync((PropostaTutor)null!);

            _repositorioPropostaMock
                .Setup(r => r.ObterTutorTurmasPorTutorId(It.IsAny<long>()))
                .ReturnsAsync(new List<PropostaTutorTurma>());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _repositorioPropostaMock
                .Setup(r => r.InserirPropostaTutor(It.IsAny<long>(), It.IsAny<PropostaTutor>()))
                .Returns(Task.CompletedTask);

            _repositorioPropostaMock
                .Setup(r => r.InserirPropostaTutorTurma(It.IsAny<long>(), It.IsAny<IEnumerable<PropostaTutorTurma>>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().Be(tutorMapeado.Id);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);

            _repositorioPropostaMock.Verify(r => r.InserirPropostaTutor(propostaId, tutorMapeado), Times.Once);
        }

        [Fact]
        public async Task DadoTutorExistenteComAlteracoesDuranteInscricao_QuandoHandle_EntaoDeveAtualizarApenasQuandoHouverMudancasRelevantes()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var tutorExistente = CriarPropostaTutor();
            var command = new SalvarPropostaTutorCommand(propostaId, new PropostaTutorDTO
            {
                Id = tutorExistente.Id,
                NomeTutor = tutorExistente.NomeTutor,
                RegistroFuncional = tutorExistente.RegistroFuncional,
                Cpf = tutorExistente.Cpf,
                ProfissionalRedeMunicipal = tutorExistente.ProfissionalRedeMunicipal,
                Turmas = new[] { new PropostaTutorTurmaDTO { TurmaId = 100, Nome = "Turma A" } }
            });

            var tutorMapeado = CriarPropostaTutor();
            tutorMapeado.Id = tutorExistente.Id;
            tutorMapeado.PropostaId = tutorExistente.PropostaId;
            tutorMapeado.Turmas = new[]
            {
                new PropostaTutorTurma { Id = 1, TurmaId = 100, PropostaTutorId = tutorExistente.Id }
            };

            var turmaAntes = new PropostaTutorTurma { Id = 1, TurmaId = 100, PropostaTutorId = tutorExistente.Id };

            _mapperMock
                .Setup(m => m.Map<PropostaTutor>(command.PropostaTutorDto))
                .Returns(tutorMapeado);

            _repositorioPropostaMock
                .Setup(r => r.ObterPropostaTutorPorId(tutorExistente.Id))
                .ReturnsAsync(tutorExistente);

            _repositorioPropostaMock
                .Setup(r => r.ObterTutorTurmasPorTutorId(tutorExistente.Id))
                .ReturnsAsync(new[] { turmaAntes });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultado.Should().Be(tutorExistente.Id);

            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _repositorioPropostaMock.Verify(r => r.AtualizarPropostaTutor(It.IsAny<PropostaTutor>()), Times.Never);
        }

        private SalvarPropostaTutorCommand CriarCommand(long propostaId, long tutorId = 0)
        {
            return new SalvarPropostaTutorCommand(propostaId, new PropostaTutorDTO
            {
                Id = tutorId,
                NomeTutor = _faker.Person.FullName,
                RegistroFuncional = _faker.Random.ReplaceNumbers("######"),
                Cpf = "12345678909",
                ProfissionalRedeMunicipal = true,
                Turmas = new[] { new PropostaTutorTurmaDTO { TurmaId = 1, Nome = "Turma A" } }
            });
        }

        private PropostaTutor CriarPropostaTutor()
        {
            return new PropostaTutor
            {
                Id = _faker.Random.Long(1, 1000),
                PropostaId = _faker.Random.Long(1, 1000),
                NomeTutor = _faker.Person.FullName,
                RegistroFuncional = _faker.Random.ReplaceNumbers("######"),
                Cpf = "12345678901",
                ProfissionalRedeMunicipal = true,
                Turmas = new[]
                {
                    new PropostaTutorTurma { Id = 1, TurmaId = 100, PropostaTutorId = 1 }
                }
            };
        }
    }
}
