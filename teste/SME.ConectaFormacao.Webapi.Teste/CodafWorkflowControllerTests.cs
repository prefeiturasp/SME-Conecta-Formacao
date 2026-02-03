using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafWorkflowControllerTests
    {
        private readonly Mock<ICasoDeUsoEnviarParaDfCodafListaPresenca> _mockCasoDeUsoEnviarParaDfCodafListaPresenca;
        private readonly Mock<ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca> _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca;
        private readonly CodafWorkflowController _controller;
        private readonly Faker _faker;

        public CodafWorkflowControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca = mocker.GetMock<ICasoDeUsoEnviarParaDfCodafListaPresenca>();
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca = mocker.GetMock<ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca>();
            _controller = mocker.CreateInstance<CodafWorkflowController>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmIdCodafListaPresenca_QuandoChamarEnviarParaDf_EntaoDeveChamarCasoDeUsoEnviarParaDfCodafListaPresenca()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1);
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.EnviarParaDf(codafListaPresencaId);
            // Assert
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca.Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }

        [Fact]
        public async Task DadoUmIdCodafListaPresencaEJustificativa_QuandoChamarDevolverParaCorrecao_EntaoDeveChamarCasoDeUsoDevolverParaCorrecaoCodafListaPresenca()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1);
            var justificativa = _faker.Lorem.Sentence();
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca
                .Setup(x => x.ExecutarAsync(codafListaPresencaId, justificativa))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.DevolverParaCorrecao(codafListaPresencaId, justificativa);
            // Assert
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca.Verify(x => x.ExecutarAsync(codafListaPresencaId, justificativa), Times.Once);
        }
    }
}
