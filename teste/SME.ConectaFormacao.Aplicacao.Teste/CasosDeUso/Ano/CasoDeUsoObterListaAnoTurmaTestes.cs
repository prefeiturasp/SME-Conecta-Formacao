using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ano;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Ano;
using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Ano
{
    public class CasoDeUsoObterListaAnoTurmaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterListaAnoTurma _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterListaAnoTurmaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterListaAnoTurma>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroAnoTurmaValido_QuandoExecutar_EntaoDeveEnviarQueryERetornarLista()
        {
            // Arrange
            var filtro = new FiltroAnoTurmaDTO
            {
                AnoLetivo = _faker.Random.Int(2020, 2030),
                Modalidade = new[] { Modalidade.EducacaoInfantil, Modalidade.Fundamental },
                ExibirOpcaoTodos = true
            };

            var retornoEsperado = new List<RetornoListagemTodosDTO>
            {
                new() { Id = 1, Descricao = "1º Ano", Todos = false },
                new() { Id = 2, Descricao = "2º Ano", Todos = false }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterAnosPorModalidadeAnoLetivoQuery>(q =>
                        q.AnoLetivo == filtro.AnoLetivo &&
                        q.Modalidade == filtro.Modalidade &&
                        q.ExibirTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(retornoEsperado);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterAnosPorModalidadeAnoLetivoQuery>(q =>
                        q.AnoLetivo == filtro.AnoLetivo &&
                        q.Modalidade == filtro.Modalidade &&
                        q.ExibirTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoConsultaSemRegistros_QuandoExecutar_EntaoDeveRetornarVazio()
        {
            // Arrange
            var filtro = new FiltroAnoTurmaDTO
            {
                AnoLetivo = _faker.Random.Int(2020, 2030),
                Modalidade = new[] { Modalidade.Medio },
                ExibirOpcaoTodos = false
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAnosPorModalidadeAnoLetivoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<RetornoListagemTodosDTO>());

            // Act
            var resultado = await _sut.Executar(filtro);

            // Assert
            resultado.Should().NotBeNull().And.BeEmpty();

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterAnosPorModalidadeAnoLetivoQuery>(q =>
                        q.AnoLetivo == filtro.AnoLetivo &&
                        q.Modalidade == filtro.Modalidade &&
                        q.ExibirTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var filtro = new FiltroAnoTurmaDTO
            {
                AnoLetivo = _faker.Random.Int(2020, 2030),
                Modalidade = new[] { Modalidade.EJA },
                ExibirOpcaoTodos = false
            };

            var mensagemErro = _faker.Lorem.Sentence();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAnosPorModalidadeAnoLetivoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var acao = () => _sut.Executar(filtro);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);

            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterAnosPorModalidadeAnoLetivoQuery>(q =>
                        q.AnoLetivo == filtro.AnoLetivo &&
                        q.Modalidade == filtro.Modalidade &&
                        q.ExibirTodos == filtro.ExibirOpcaoTodos),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
