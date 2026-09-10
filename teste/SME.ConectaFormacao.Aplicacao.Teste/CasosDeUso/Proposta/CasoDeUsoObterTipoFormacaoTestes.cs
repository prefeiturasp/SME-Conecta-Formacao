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
    public class CasoDeUsoObterTipoFormacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterTipoFormacao _casoDeUso;

        public CasoDeUsoObterTipoFormacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterTipoFormacao>();
        }

        [Fact]
        public async Task DadoTiposFormacaoExistentes_QuandoExecutar_EntaoDeveEnviarQueryERetornarLista()
        {
            // Arrange
            var tiposFormacaoEsperados = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Curso" },
                new() { Id = 2, Descricao = "Seminário" },
                new() { Id = 3, Descricao = "Palestra" }
            };

            _mediatorMock
                .Setup(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposFormacaoEsperados);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(tiposFormacaoEsperados);

            _mediatorMock.Verify(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoTiposFormacaoInexistentes_QuandoExecutar_EntaoDeveRetornarColecaoVazia()
        {
            // Arrange
            var tiposFormacaoEsperados = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tiposFormacaoEsperados);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _mediatorMock.Verify(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoEnviarQuery_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            const string mensagemErro = "Erro ao obter tipos de formação";

            _mediatorMock
                .Setup(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _casoDeUso.Executar();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage(mensagemErro);

            _mediatorMock.Verify(m => m.Send(ObterTipoFormacaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
