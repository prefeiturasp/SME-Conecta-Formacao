using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterSePropostaPossuiCodaf;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterSePropostaPossuiCodafQueryHandlerTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioCodafCursoNaoHomologadoMock;
        private readonly ObterSePropostaPossuiCodafQueryHandler _sut;

        public ObterSePropostaPossuiCodafQueryHandlerTestes()
        {
            var autoMocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = autoMocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafCursoNaoHomologadoMock = autoMocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _sut = autoMocker.CreateInstance<ObterSePropostaPossuiCodafQueryHandler>();
        }

        [Fact]
        public async Task DadoPropostaComCodafListaPresenca_QuandoHandle_EntaoRetornaTrueENaoConsultaCursoNaoHomologado()
        {
            // Arrange
            const long propostaId = 123;
            _repositorioCodafListaPresencaMock
                .Setup(r => r.PossuiPorPropostaIdAsync(propostaId))
                .ReturnsAsync(true);

            var query = new ObterSePropostaPossuiCodafQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.PossuiPorPropostaIdAsync(propostaId), Times.Once);
            _repositorioCodafCursoNaoHomologadoMock.Verify(r => r.PossuiPorPropostaIdAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaComCodafCursoNaoHomologado_QuandoHandle_EntaoRetornaTrue()
        {
            // Arrange
            const long propostaId = 123;
            _repositorioCodafListaPresencaMock
                .Setup(r => r.PossuiPorPropostaIdAsync(propostaId))
                .ReturnsAsync(false);

            _repositorioCodafCursoNaoHomologadoMock
                .Setup(r => r.PossuiPorPropostaIdAsync(propostaId))
                .ReturnsAsync(true);

            var query = new ObterSePropostaPossuiCodafQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.PossuiPorPropostaIdAsync(propostaId), Times.Once);
            _repositorioCodafCursoNaoHomologadoMock.Verify(r => r.PossuiPorPropostaIdAsync(propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaSemNenhumCodaf_QuandoHandle_EntaoRetornaFalse()
        {
            // Arrange
            const long propostaId = 123;
            _repositorioCodafListaPresencaMock
                .Setup(r => r.PossuiPorPropostaIdAsync(propostaId))
                .ReturnsAsync(false);

            _repositorioCodafCursoNaoHomologadoMock
                .Setup(r => r.PossuiPorPropostaIdAsync(propostaId))
                .ReturnsAsync(false);

            var query = new ObterSePropostaPossuiCodafQuery(propostaId);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();
            _repositorioCodafListaPresencaMock.Verify(r => r.PossuiPorPropostaIdAsync(propostaId), Times.Once);
            _repositorioCodafCursoNaoHomologadoMock.Verify(r => r.PossuiPorPropostaIdAsync(propostaId), Times.Once);
        }
    }
}
