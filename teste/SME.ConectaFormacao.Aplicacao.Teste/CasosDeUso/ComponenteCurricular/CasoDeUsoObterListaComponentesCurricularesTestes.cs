using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.ComponenteCurricular
{
    public class CasoDeUsoObterListaComponentesCurricularesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterListaComponentesCurriculares _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterListaComponentesCurricularesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterListaComponentesCurriculares>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroComponenteCurricularValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarLista()
        {
            // Arrange
            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new[] { _faker.Random.Long(1, 100), _faker.Random.Long(101, 200) },
                ExibirOpcaoTodos = true
            };

            var retornoEsperado = new List<RetornoListagemTodosDTO>
            {
                new() { Id = 1, Descricao = "Matemática", Todos = false },
                new() { Id = 2, Descricao = "Português", Todos = false }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterComponentesCurricularesPorAnoTurmaQuery>(q =>
                        q.AnoTurmaId == filtro.AnoTurmaId &&
                        q.ExibirOpcaoTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(retornoEsperado);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterComponentesCurricularesPorAnoTurmaQuery>(q =>
                        q.AnoTurmaId == filtro.AnoTurmaId &&
                        q.ExibirOpcaoTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoConsultaSemRegistros_QuandoExecutar_EntaoDeveRetornarVazio()
        {
            // Arrange
            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new[] { _faker.Random.Long(1, 100) },
                ExibirOpcaoTodos = false
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterComponentesCurricularesPorAnoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<RetornoListagemTodosDTO>());

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull().And.BeEmpty();

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterComponentesCurricularesPorAnoTurmaQuery>(q =>
                        q.AnoTurmaId == filtro.AnoTurmaId &&
                        q.ExibirOpcaoTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new[] { _faker.Random.Long(1, 100) },
                ExibirOpcaoTodos = false
            };

            var mensagemErro = _faker.Lorem.Sentence();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterComponentesCurricularesPorAnoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var acao = () => _sut.Executar(filtro);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterComponentesCurricularesPorAnoTurmaQuery>(q =>
                        q.AnoTurmaId == filtro.AnoTurmaId &&
                        q.ExibirOpcaoTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
