using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoArquivo;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.ImportacaoInscricoes
{
    public class CasoDeUsoInscricaoManualCancelarProcessamentoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoInscricaoManualCancelarProcessamento _sut;

        public CasoDeUsoInscricaoManualCancelarProcessamentoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoInscricaoManualCancelarProcessamento>();
        }

        [Fact]
        public async Task DadoArquivoIdValido_QuandoExecutar_EntaoDeveEnviarComandoParaMediatorERetornarResultado()
        {
            // Arrange
            var arquivoImportacaoId = 123L;
            var resultadoEsperado = true;
            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarSituacaoArquivosParaCanceladoCommand>(c => c.ArquivoImportacaoId == arquivoImportacaoId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.Executar(arquivoImportacaoId);

            // Assert
            resultado.Should().Be(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarSituacaoArquivosParaCanceladoCommand>(c => c.ArquivoImportacaoId == arquivoImportacaoId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
