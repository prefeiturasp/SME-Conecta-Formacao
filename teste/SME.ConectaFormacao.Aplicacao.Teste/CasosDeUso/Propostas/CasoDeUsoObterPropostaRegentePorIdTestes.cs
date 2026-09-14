using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentePorId;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegenteTurmaPorRegenteId;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;
using EntidadePropostaRegente = SME.ConectaFormacao.Dominio.Entidades.PropostaRegente;
using EntidadePropostaRegenteTurma = SME.ConectaFormacao.Dominio.Entidades.PropostaRegenteTurma;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterPropostaRegentePorIdTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterPropostaRegentePorId _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterPropostaRegentePorIdTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoObterPropostaRegentePorId>();
            _faker = new Faker();
        }

        [Fact]
        public void DadoMapperNulo_QuandoInstanciar_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            var mediator = _mediatorMock.Object;

            // Act
            var act = () => new CasoDeUsoObterPropostaRegentePorId(mediator, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }

        [Fact]
        public async Task DadoRegenteIdValidoERegenteExistente_QuandoExecutar_EntaoDeveRetornarRegenteComTurmas()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);
            var regenteEntidade = new EntidadePropostaRegente
            {
                Id = regenteId,
                NomeRegente = _faker.Person.FullName,
                Cpf = _faker.Random.Replace("###.###.###-##"),
                ProfissionalRedeMunicipal = true,
                RegistroFuncional = _faker.Random.Replace("#######")
            };

            var turmasEntidade = new List<EntidadePropostaRegenteTurma>
            {
                new() { Id = 1, PropostaRegenteId = regenteId, TurmaId = 10 },
                new() { Id = 2, PropostaRegenteId = regenteId, TurmaId = 20 }
            };

            var regenteDto = new PropostaRegenteDTO
            {
                Id = regenteId,
                NomeRegente = regenteEntidade.NomeRegente,
                Cpf = regenteEntidade.Cpf,
                ProfissionalRedeMunicipal = regenteEntidade.ProfissionalRedeMunicipal,
                RegistroFuncional = regenteEntidade.RegistroFuncional
            };

            var turmasDto = new List<PropostaRegenteTurmaDTO>
            {
                new() { TurmaId = 10, Nome = "Turma 1" },
                new() { TurmaId = 20, Nome = "Turma 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(regenteEntidade);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegenteTurmaPorRegenteIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEntidade);

            _mapperMock
                .Setup(m => m.Map<PropostaRegenteDTO>(regenteEntidade))
                .Returns(regenteDto);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PropostaRegenteTurmaDTO>>(turmasEntidade))
                .Returns(turmasDto);

            // Act
            var resultado = await _sut.Executar(regenteId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(regenteDto);
            resultado.Turmas.Should().BeEquivalentTo(turmasDto);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegenteTurmaPorRegenteIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);

            _mapperMock.Verify(m => m.Map<PropostaRegenteDTO>(regenteEntidade), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<PropostaRegenteTurmaDTO>>(turmasEntidade), Times.Once);
        }

        [Fact]
        public async Task DadoRegenteNaoEncontrado_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntidadePropostaRegente)null!);

            // Act
            var act = async () => await _sut.Executar(regenteId);

            // Assert
            var exception = await act.Should().ThrowAsync<NegocioException>()
                .WithMessage("Registro não encontrado");
            exception.Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterRegenteTurmaPorRegenteIdQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);

            _mapperMock.Verify(m => m.Map<PropostaRegenteDTO>(It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoConsultarRegente_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(regenteId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<ObterRegenteTurmaPorRegenteIdQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoConsultarTurmas_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var regenteId = _faker.Random.Long(1, 1000);
            var regenteEntidade = new EntidadePropostaRegente { Id = regenteId };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(regenteEntidade);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterRegenteTurmaPorRegenteIdQuery>(q => q.RegenteId == regenteId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _sut.Executar(regenteId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegentePorIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterRegenteTurmaPorRegenteIdQuery>(q => q.RegenteId == regenteId),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
