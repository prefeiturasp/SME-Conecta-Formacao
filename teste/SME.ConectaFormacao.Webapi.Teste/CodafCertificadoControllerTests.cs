using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafCertificadoControllerTests
    {
        private readonly Mock<ICasoDeUsoEmitirCertificadoCodaf> _mockCasoDeUsoEmitirCertificadoCodaf;
        private readonly Mock<ICasoDeUsoListarCertificadoCodafUsuario> _mockCasoDeUsoListarCertificadoCodafUsuario;
        private readonly Mock<ICasoDeUsoObterCertificadoCodafParaDownload> _mockCasoDeUsoObterCertificadoCodafParaDownload;
        private readonly CodafCertificadoController _controller;
        private readonly Faker _faker;

        public CodafCertificadoControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoEmitirCertificadoCodaf = mocker.GetMock<ICasoDeUsoEmitirCertificadoCodaf>();
            _mockCasoDeUsoListarCertificadoCodafUsuario = mocker.GetMock<ICasoDeUsoListarCertificadoCodafUsuario>();
            _mockCasoDeUsoObterCertificadoCodafParaDownload = mocker.GetMock<ICasoDeUsoObterCertificadoCodafParaDownload>();
            _controller = mocker.CreateInstance<CodafCertificadoController>();
            _faker = new();
        }

        [Fact]
        public async Task DadoCodafListaPresencaId_EmitirCertificadosCodaf_EntaoDeveChamarCasoDeUsoEmitir()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1, long.MaxValue);
            _mockCasoDeUsoEmitirCertificadoCodaf
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            await _controller.EmitirCertificadosCodaf(codafListaPresencaId);

            // Assert
            _mockCasoDeUsoEmitirCertificadoCodaf.Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }

        [Fact]
        public async Task DadoFiltro_ListarCertificadosUsuario_EntaoDeveChamarCasoDeUsoListar()
        {
            // Arrange
            var filtro = new FiltroListaCertificadoCodafDto() { NumeroPagina = 1, NumeroRegistros = 10 };
            _mockCasoDeUsoListarCertificadoCodafUsuario
                .Setup(x => x.ExecutarAsync(filtro))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<ListagemResultadoCertificadoCodafUsuarioDto>>.DeSucesso(
                    new PaginacaoResultadoDto<ListagemResultadoCertificadoCodafUsuarioDto>([], 0, 0)));
            // Act
            await _controller.ListarCertificadosUsuario(filtro);
            // Assert
            _mockCasoDeUsoListarCertificadoCodafUsuario.Verify(x => x.ExecutarAsync(filtro), Times.Once);
        }

        [Fact]
        public async Task DadoCertificadoCodafId_ObterCertificadoParaDownload_EntaoDeveChamarCasoDeUsoObter()
        {
            // Arrange
            var certificadoCodafId = _faker.Random.Long(1, long.MaxValue);
            _mockCasoDeUsoObterCertificadoCodafParaDownload
                .Setup(x => x.ExecutarAsync(certificadoCodafId))
                .ReturnsAsync(Resultado<CodafCertificadoParaDownloadDto>.DeSucesso(
                    new CodafCertificadoParaDownloadDto()));
            // Act
            await _controller.ObterCertificadoParaDownload(certificadoCodafId);
            // Assert
            _mockCasoDeUsoObterCertificadoCodafParaDownload.Verify(x => x.ExecutarAsync(certificadoCodafId), Times.Once);
        }
    }
}