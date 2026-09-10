using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Modalidade;
using SME.ConectaFormacao.Aplicacao.Dtos;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterModalidadeTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterModalidade _sut;

        public CasoDeUsoObterModalidadeTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterModalidade>();
        }

        [Fact]
        public async Task DadoModalidadesExistentes_QuandoExecutar_EntaoDeveEnviarInstanciaQueryERetornarListaModalidades()
        {
            // Arrange
            var modalidadesEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Presencial" },
                new() { Id = 2, Descricao = "EAD" },
                new() { Id = 3, Descricao = "Híbrido" }
            };

            _mediatorMock
                .Setup(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(modalidadesEsperadas);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(modalidadesEsperadas);
            _mediatorMock.Verify(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoNenhumaModalidade_QuandoExecutar_EntaoDeveRetornarVazio()
        {
            // Arrange
            var modalidadesVazia = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(modalidadesVazia);

            // Act
            var resultado = await _sut.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mediatorMock.Verify(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            const string mensagemErro = "Erro ao obter modalidades";

            _mediatorMock
                .Setup(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            Func<Task> acao = () => _sut.Executar();

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(ObterModalidadesQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
