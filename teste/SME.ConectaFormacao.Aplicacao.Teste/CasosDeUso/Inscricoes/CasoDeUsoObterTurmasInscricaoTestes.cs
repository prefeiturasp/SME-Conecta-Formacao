using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterTurmasInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTurmasInscricao _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterTurmasInscricaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoObterTurmasInscricao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdECodigoDrePreenchidos_QuandoExecutar_EntaoDeveEnviarQueryComParametrosERetornarListaDeTurmas()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var codigoDre = _faker.Random.AlphaNumeric(6);
            var turmasEsperadas = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = _faker.Random.Long(1, 100), Descricao = _faker.Company.CatchPhrase() },
                new RetornoListagemDTO { Id = _faker.Random.Long(101, 200), Descricao = _faker.Company.CatchPhrase() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                    q.PropostaId == propostaId &&
                    q.CodigoDre == codigoDre
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEsperadas);

            // Act
            var resultado = await _sut.Executar(propostaId, codigoDre);

            // Assert
            resultado.Should().BeEquivalentTo(turmasEsperadas);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                q.PropostaId == propostaId &&
                q.CodigoDre == codigoDre
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoCodigoDreNulo_QuandoExecutar_EntaoDeveEnviarQueryComCodigoDreNuloERetornarListaDeTurmas()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var turmasEsperadas = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = _faker.Random.Long(1, 100), Descricao = _faker.Company.CatchPhrase() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                    q.PropostaId == propostaId &&
                    q.CodigoDre == null
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEsperadas);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().BeEquivalentTo(turmasEsperadas);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                q.PropostaId == propostaId &&
                q.CodigoDre == null
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoQueryRetornarVazio_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var codigoDre = _faker.Random.AlphaNumeric(6);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                    q.PropostaId == propostaId &&
                    q.CodigoDre == codigoDre
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<RetornoListagemDTO>());

            // Act
            var resultado = await _sut.Executar(propostaId, codigoDre);

            // Assert
            resultado.Should().BeEmpty();
            _mediatorMock.Verify(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                q.PropostaId == propostaId &&
                q.CodigoDre == codigoDre
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmasComVagasPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(propostaId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterPropostaTurmasComVagasPorIdQuery>(q =>
                q.PropostaId == propostaId &&
                q.CodigoDre == null
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
