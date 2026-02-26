using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCertificados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafCertificadoControllerTests
    {
        private readonly Mock<ICasoDeUsoEmitirCertificadoCodaf> _mockCasoDeUsoEmitirCertificadoCodaf;
        private readonly Mock<ICasoDeUsoListarMeusCertificadosCodaf> _mockCasoDeUsoListarMeusCertificadosCodaf;
        private readonly Mock<ICasoDeUsoObterCertificadoCodafParaDownload> _mockCasoDeUsoObterCertificadoCodafParaDownload;
        private readonly Mock<ICasoDeUsoListarTodosCertificadosCodaf> _mockCasoDeUsoListarTodosCertificadosCodaf;
        private readonly Mock<ICasoDeUsoDownloadLoteCertificados> _mockCasoDeUsoDownloadLoteCertificados;
        private readonly CodafCertificadoController _controller;
        private readonly Faker _faker;

        public CodafCertificadoControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoEmitirCertificadoCodaf = mocker.GetMock<ICasoDeUsoEmitirCertificadoCodaf>();
            _mockCasoDeUsoListarMeusCertificadosCodaf = mocker.GetMock<ICasoDeUsoListarMeusCertificadosCodaf>();
            _mockCasoDeUsoObterCertificadoCodafParaDownload = mocker.GetMock<ICasoDeUsoObterCertificadoCodafParaDownload>();
            _mockCasoDeUsoListarTodosCertificadosCodaf = mocker.GetMock<ICasoDeUsoListarTodosCertificadosCodaf>();
            _mockCasoDeUsoDownloadLoteCertificados = mocker.GetMock<ICasoDeUsoDownloadLoteCertificados>();

            _controller = mocker.CreateInstance<CodafCertificadoController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafListaPresencaId_QuandoEmitirCertificadosCodaf_EntaoDeveChamarCasoDeUsoEmitir()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1, long.MaxValue);

            _mockCasoDeUsoEmitirCertificadoCodaf
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(Resultado.DeSucesso());

            // Act
            var resultado = await _controller.EmitirCertificadosCodaf(codafListaPresencaId);

            // Assert
            _mockCasoDeUsoEmitirCertificadoCodaf.Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
            Assert.IsType<NoContentResult>(resultado);
        }

        [Fact]
        public async Task DadoFiltro_QuandoListarMeusCertificados_EntaoDeveChamarCasoDeUsoListarMeusCertificados()
        {
            // Arrange
            var filtro = new FiltroListaMeusCertificadosCodafDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var retornoPaginado = new PaginacaoResultadoDto<MeusCertificadosCodafDto>([], 0, 10);

            _mockCasoDeUsoListarMeusCertificadosCodaf
                .Setup(x => x.ExecutarAsync(filtro))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<MeusCertificadosCodafDto>>.DeSucesso(retornoPaginado));

            // Act
            var resultado = await _controller.ListarMeusCertificados(filtro);

            // Assert
            _mockCasoDeUsoListarMeusCertificadosCodaf.Verify(x => x.ExecutarAsync(filtro), Times.Once);
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoCertificadoCodafId_QuandoObterCertificadoParaDownload_EntaoDeveChamarCasoDeUsoObterDownload()
        {
            // Arrange
            var certificadoCodafId = _faker.Random.Long(1, long.MaxValue);
            var retornoDto = new CodafCertificadoParaDownloadDto();

            _mockCasoDeUsoObterCertificadoCodafParaDownload
                .Setup(x => x.ExecutarAsync(certificadoCodafId))
                .ReturnsAsync(Resultado<CodafCertificadoParaDownloadDto>.DeSucesso(retornoDto));

            // Act
            var resultado = await _controller.ObterCertificadoParaDownload(certificadoCodafId);

            // Assert
            _mockCasoDeUsoObterCertificadoCodafParaDownload.Verify(x => x.ExecutarAsync(certificadoCodafId), Times.Once);
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoFiltro_QuandoListarTodosCertificados_EntaoDeveChamarCasoDeUsoListarTodosCertificados()
        {
            // Arrange
            var filtro = new FiltroListaTodosCertificadosCodafDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var retornoPaginado = new PaginacaoResultadoDto<ListagemCertificadosCodafDto>([], 0, 10);

            _mockCasoDeUsoListarTodosCertificadosCodaf
                .Setup(x => x.ExecutarAsync(filtro))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<ListagemCertificadosCodafDto>>.DeSucesso(retornoPaginado));

            // Act
            var resultado = await _controller.ListarTodosCertificados(filtro);

            // Assert
            _mockCasoDeUsoListarTodosCertificadosCodaf.Verify(x => x.ExecutarAsync(filtro), Times.Once);
            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task DadoIdsValidos_QuandoDownloadLoteCertificados_EntaoDeveConfigurarResponseEChamarCasoDeUso()
        {
            // Arrange
            var ids = new List<long> { _faker.Random.Long(1, 100), _faker.Random.Long(1, 100) };
            var cancellationToken = CancellationToken.None;

            var httpContext = new DefaultHttpContext();
            var featureMock = new Mock<IHttpBodyControlFeature>();
            httpContext.Features.Set(featureMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockCasoDeUsoDownloadLoteCertificados
                .Setup(x => x.ExecutarAsync(ids, It.IsAny<Stream>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.DownloadLoteCertificados(ids, cancellationToken);

            // Assert
            featureMock.VerifySet(f => f.AllowSynchronousIO = true, Times.Once);
            Assert.Equal("application/zip", _controller.Response.ContentType);
            Assert.True(_controller.Response.Headers.ContainsKey("Content-Disposition"));
            _mockCasoDeUsoDownloadLoteCertificados.Verify(x => x.ExecutarAsync(ids, It.IsAny<Stream>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DadoHttpBodyControlFeatureNulo_QuandoDownloadLoteCertificados_EntaoDeveConfigurarResponseSemErroEChamarCasoDeUso()
        {
            // Arrange
            var ids = new List<long> { _faker.Random.Long(1, 100) };
            var cancellationToken = CancellationToken.None;

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _mockCasoDeUsoDownloadLoteCertificados
                .Setup(x => x.ExecutarAsync(ids, It.IsAny<Stream>(), cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.DownloadLoteCertificados(ids, cancellationToken);

            // Assert
            Assert.Equal("application/zip", _controller.Response.ContentType);
            Assert.True(_controller.Response.Headers.ContainsKey("Content-Disposition"));
            _mockCasoDeUsoDownloadLoteCertificados.Verify(x => x.ExecutarAsync(ids, It.IsAny<Stream>(), cancellationToken), Times.Once);
        }
    }
}