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
    public class SalvarPropostaTutorCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarPropostaTutorCommandHandler _sut;
        private readonly Faker _faker;

        public SalvarPropostaTutorCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<SalvarPropostaTutorCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoTutorComCpfInvalido_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange
            var dto = new PropostaTutorDTO { Id = 1, Cpf = "123" };
            var comando = new SalvarPropostaTutorCommand(1, dto);

            var tutorMapeado = new PropostaTutor { Id = 1, Cpf = "123" };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutor>(dto))
                .Returns(tutorMapeado);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoNovoTutor_QuandoExecutar_EntaoDeveInserirESalvarTurmas()
        {
            // Arrange
            var dto = new PropostaTutorDTO
            {
                Id = 0,
                Cpf = _faker.Person.Cpf(),
                Turmas = [new() { TurmaId = 1 }]
            };
            var comando = new SalvarPropostaTutorCommand(1, dto);

            var tutorMapeado = new PropostaTutor
            {
                Id = 0,
                Cpf = dto.Cpf,
                Turmas = [new() { TurmaId = 1, Id = 0 }]
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutor>(dto))
                .Returns(tutorMapeado);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterTutorTurmasPorTutorId(0))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirPropostaTutor(1, tutorMapeado), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.InserirPropostaTutorTurma(0, It.Is<IEnumerable<PropostaTutorTurma>>(t => t.Count() == 1)), Times.Once);
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.AtualizarPropostaTutor(It.IsAny<PropostaTutor>()), Times.Never);
            transacaoMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoTutorExistenteComAlteracao_QuandoExecutar_EntaoDeveAtualizar()
        {
            // Arrange
            var dto = new PropostaTutorDTO { Id = 1, NomeTutor = "Novo Nome" };
            var comando = new SalvarPropostaTutorCommand(1, dto);

            var tutorAntes = new PropostaTutor { Id = 1, NomeTutor = "Nome Antigo" };
            var tutorDepois = new PropostaTutor { Id = 1, NomeTutor = "Novo Nome", Turmas = new List<PropostaTutorTurma>() };

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterPropostaTutorPorId(1))
                .ReturnsAsync(tutorAntes);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutor>(dto))
                .Returns(tutorDepois);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterTutorTurmasPorTutorId(1))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            // Act
            await _sut.Handle(comando, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioProposta>().Verify(m => m.AtualizarPropostaTutor(tutorDepois), Times.Once);
            transacaoMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoBanco_QuandoExecutar_EntaoDeveFazerRollback()
        {
            // Arrange
            var dto = new PropostaTutorDTO { Id = 0 };
            var comando = new SalvarPropostaTutorCommand(1, dto);
            var tutorDepois = new PropostaTutor { Id = 0, Turmas = new List<PropostaTutorTurma>() };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutor>(dto))
                .Returns(tutorDepois);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.ObterTutorTurmasPorTutorId(0))
                .ReturnsAsync([]);

            var transacaoMock = new Mock<System.Data.IDbTransaction>();
            _mocker.GetMock<ITransacao>().Setup(m => m.Iniciar()).Returns(transacaoMock.Object);

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(m => m.InserirPropostaTutor(1, tutorDepois))
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
