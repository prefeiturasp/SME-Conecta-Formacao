using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Dre;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterListaDreTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterListaDre _sut;

        public CasoDeUsoObterListaDreTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterListaDre>();
        }

        [Fact]
        public async Task DadoExibirTodosVerdadeiro_QuandoExecutar_EntaoDeveEnviarQueryComTrueERetornarListaDre()
        {
            // Arrange
            const bool exibirTodos = true;
            var dresEsperadas = new List<DreDTO>
            {
                new() { Id = 1, Codigo = "108100", Descricao = "DRE Butantã" },
                new() { Id = 2, Codigo = "108200", Descricao = "DRE Campo Limpo" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterListaDreQuery>(q => q.ExibirTodos == exibirTodos), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dresEsperadas);

            // Act
            var resultado = await _sut.Executar(exibirTodos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(dresEsperadas);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterListaDreQuery>(q => q.ExibirTodos == exibirTodos), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExibirTodosFalso_QuandoExecutar_EntaoDeveEnviarQueryComFalseERetornarListaDre()
        {
            // Arrange
            const bool exibirTodos = false;
            var dresEsperadas = new List<DreDTO>
            {
                new() { Id = 1, Codigo = "108100", Descricao = "DRE Butantã" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterListaDreQuery>(q => q.ExibirTodos == exibirTodos), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dresEsperadas);

            // Act
            var resultado = await _sut.Executar(exibirTodos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(dresEsperadas);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterListaDreQuery>(q => q.ExibirTodos == exibirTodos), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFalhaNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            const bool exibirTodos = true;
            const string mensagemErro = "Erro ao obter lista de DREs";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterListaDreQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            Func<Task> acao = () => _sut.Executar(exibirTodos);

            // Assert
            await acao.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterListaDreQuery>(q => q.ExibirTodos == exibirTodos), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
