using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterPropostaTutorPorIdTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoObterPropostaTutorPorId _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaTutorPorIdTestes()
        {
            _mocker = new AutoMocker();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoObterPropostaTutorPorId>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoTutorExistenteComTurmas_QuandoExecutar_EntaoDeveRetornarDtoComTurmasMapeadas()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1);

            var tutorEntidade = new PropostaTutor
            {
                Id = tutorId,
                NomeTutor = _faker.Person.FullName,
                Cpf = _faker.Person.Cpf(),
                PropostaId = _faker.Random.Long(1)
            };

            var turmasEntidade = new List<PropostaTutorTurma>
            {
                new() { TurmaId = 10, PropostaTutorId = tutorId },
                new() { TurmaId = 20, PropostaTutorId = tutorId }
            };

            var tutorDtoEsperado = new PropostaTutorDTO
            {
                NomeTutor = tutorEntidade.NomeTutor
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterTutorPorIdQuery>(q => q.TutorId == tutorId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tutorEntidade);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterTutorTurmaPorTutorIdQuery>(q => q.TutorId == tutorId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEntidade);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutorDTO>(tutorEntidade))
                .Returns(tutorDtoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(tutorId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tutorEntidade.NomeTutor, resultado.NomeTutor);
            Assert.NotNull(resultado.Turmas);
            Assert.Equal(2, resultado.Turmas.Count());
            Assert.Contains(resultado.Turmas, t => t.TurmaId == 10);

            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<ObterTutorPorIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<ObterTutorTurmaPorTutorIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTutorExistenteSemTurmas_QuandoExecutar_EntaoDeveRetornarDtoSemTurmasPreenchidas()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1);
            var tutorEntidade = new PropostaTutor { Id = tutorId };
            var turmasVazias = new List<PropostaTutorTurma>();
            var tutorDtoEsperado = new PropostaTutorDTO();

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterTutorPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tutorEntidade);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterTutorTurmaPorTutorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasVazias);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<PropostaTutorDTO>(tutorEntidade))
                .Returns(tutorDtoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(tutorId);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado.Turmas);

            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<ObterTutorTurmaPorTutorIdQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTutorNaoExistente_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var tutorId = _faker.Random.Long(1);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterTutorPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropostaTutor)null!);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(tutorId));

            Assert.Equal("Registro não encontrado", excecao.Message);

            // Verifica que a query de turmas NÂO foi chamada se o tutor não existe
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<ObterTutorTurmaPorTutorIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
