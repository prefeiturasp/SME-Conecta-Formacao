using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoObterTurmasPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTurmasProposta _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterTurmasPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterTurmasProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoIdValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarListaTurmas()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);
            var turmasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = _faker.Random.Word() },
                new() { Id = 2, Descricao = _faker.Random.Word() }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(turmasEsperadas);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTurmasInexistentes_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);
            var turmasEsperadas = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 10000);
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = async () => await _casoDeUso.Executar(id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterPropostaTurmasPorIdQuery>(q => q.Id == id),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
