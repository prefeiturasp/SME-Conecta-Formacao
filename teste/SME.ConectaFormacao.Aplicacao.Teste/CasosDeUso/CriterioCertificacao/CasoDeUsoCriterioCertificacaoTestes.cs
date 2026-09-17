using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Consultas.CriterioCertificacao.ObterCriterioCertificacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CriterioCertificacao
{
    public class CasoDeUsoCriterioCertificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoCriterioCertificacao _casoDeUso;

        public CasoDeUsoCriterioCertificacaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoCriterioCertificacao>();
        }

        [Fact]
        public async Task QuandoExecutar_EntaoDeveEnviarQueryParaMediatorERetornarResultado()
        {
            // Arrange
            var resultadoEsperado = new List<RetornoListagemDTO> { new RetornoListagemDTO() };
            _mediatorMock
                .Setup(m => m.Send(ObterCriterioCertificacaoQuery.Instancia, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().BeEquivalentTo(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(ObterCriterioCertificacaoQuery.Instancia, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
