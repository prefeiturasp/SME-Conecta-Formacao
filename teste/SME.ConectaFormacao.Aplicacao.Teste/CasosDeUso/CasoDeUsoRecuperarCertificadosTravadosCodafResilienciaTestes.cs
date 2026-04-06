using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoRecuperarCertificadosTravadosCodafResilienciaTestes
    {
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorioCodafCertificado;
        private readonly CasoDeUsoRecuperarCertificadosTravadosCodafResiliencia _sut;

        public CasoDeUsoRecuperarCertificadosTravadosCodafResilienciaTestes()
        {
            var mocker = new AutoMocker();
            _mockRepositorioCodafCertificado = mocker.GetMock<IRepositorioCodafCertificado>();
            _sut = mocker.CreateInstance<CasoDeUsoRecuperarCertificadosTravadosCodafResiliencia>();
        }

        [Fact]
        public async Task DadoFluxoSemErros_QuandoExecutarAsync_EntaoDeveChamarRepositorio()
        {
            // Act
            await _sut.ExecutarAsync(CancellationToken.None);
            // Assert
            _mockRepositorioCodafCertificado.Verify(r => r.RecuperarCertificadosTravadosAsync(), Times.Once);
        }

        [Fact]
        public async Task DadoFluxoComErro_QuandoExecutarAsync_EntaoNaoDeveLancarExcecao()
        {
            // Arrange
            _mockRepositorioCodafCertificado
                .Setup(r => r.RecuperarCertificadosTravadosAsync())
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act & Assert
            await _sut.ExecutarAsync(CancellationToken.None);
            _mockRepositorioCodafCertificado.Verify(r => r.RecuperarCertificadosTravadosAsync(), Times.Once);
        }
    }
}
