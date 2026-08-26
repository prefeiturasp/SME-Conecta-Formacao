using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafDeclaracaoControllerTestes
    {
        private readonly Mock<ICasoDeUsoEmitirDeclaracaoCodaf> _casoDeUsoEmitirDeclaracaoCodafMock;
        private readonly Mock<ICasoDeUsoListarMinhasDeclaracoesCodaf> _casoDeUsoListarMinhasDeclaracoesCodafMock;
        private readonly CodafDeclaracaoController _sut;
        private readonly Faker _faker;

        public CodafDeclaracaoControllerTestes()
        {
            var mocker = new AutoMocker();
            _casoDeUsoEmitirDeclaracaoCodafMock = mocker.GetMock<ICasoDeUsoEmitirDeclaracaoCodaf>();
            _casoDeUsoListarMinhasDeclaracoesCodafMock = mocker.GetMock<ICasoDeUsoListarMinhasDeclaracoesCodaf>();

            _sut = mocker.CreateInstance<CodafDeclaracaoController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoHomologadoIdValido_QuandoChamarEmitirDeclaracoesCodaf_EntaoDeveRetornarOk()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var resultadoSucesso = Resultado.DeSucesso();

            _casoDeUsoEmitirDeclaracaoCodafMock
                .Setup(c => c.ExecutarAsync(It.IsAny<long>()))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.EmitirDeclaracoesCodaf(id) as NoContentResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(204);
            _casoDeUsoEmitirDeclaracaoCodafMock.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarListarMinhasDeclaracoes_EntaoDeveRetornarOkComDados()
        {
            // Arrange
            var filtro = new FiltroListaMinhasDeclaracoesCodafDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var paginacaoDto = new PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>([], 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<MinhasDeclaracoesCodafDto>>.DeSucesso(paginacaoDto);

            _casoDeUsoListarMinhasDeclaracoesCodafMock
                .Setup(c => c.ExecutarAsync(filtro))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ListarMinhasDeclaracoes(filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(200);
            _casoDeUsoListarMinhasDeclaracoesCodafMock.Verify(c => c.ExecutarAsync(filtro), Times.Once);
        }
        [Fact]
        public async Task DadoDeclaracaoIdValido_QuandoChamarObterDeclaracaoParaDownload_EntaoDeveRetornarOkComDados()
        {
            // Arrange
            var id = 1L;
            var mocker = new AutoMocker();
            var mockCasoDeUso = mocker.GetMock<ICasoDeUsoObterDeclaracaoCodafParaDownload>();
            var sut = mocker.CreateInstance<CodafDeclaracaoController>();
            
            var dto = new CodafDeclaracaoParaDownloadDto();
            var resultadoSucesso = Resultado<CodafDeclaracaoParaDownloadDto>.DeSucesso(dto);

            mockCasoDeUso.Setup(c => c.ExecutarAsync(id)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await sut.ObterDeclaracaoParaDownload(id) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(200);
            mockCasoDeUso.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarListarTodasDeclaracoes_EntaoDeveRetornarOkComDados()
        {
            // Arrange
            var filtro = new FiltroListagemTodasDeclaracoesCodafDto { Pagina = 1, TamanhoPagina = 10 };
            var mocker = new AutoMocker();
            var mockCasoDeUso = mocker.GetMock<ICasoDeUsoListarTodasDeclaracoesCodaf>();
            var sut = mocker.CreateInstance<CodafDeclaracaoController>();
            
            var paginacaoDto = new PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>([], 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<ListagemDeclaracoesCodafDto>>.DeSucesso(paginacaoDto);

            mockCasoDeUso.Setup(c => c.ExecutarAsync(filtro)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await sut.ListarTodasDeclaracoes(filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be(200);
            mockCasoDeUso.Verify(c => c.ExecutarAsync(filtro), Times.Once);
        }

        [Fact]
        public async Task DadoIdsValidos_QuandoChamarDownloadLoteDeclaracoes_EntaoDeveExecutarComSucesso()
        {
            // Arrange
            var ids = new List<long> { 1, 2 };
            var cancellationToken = CancellationToken.None;
            var mocker = new AutoMocker();
            var mockCasoDeUso = mocker.GetMock<ICasoDeUsoDownloadLoteDeclaracoes>();
            var sut = mocker.CreateInstance<CodafDeclaracaoController>();

            var httpContextMock = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            var featuresMock = new Mock<Microsoft.AspNetCore.Http.Features.IFeatureCollection>();
            var bodyControlFeatureMock = new Mock<Microsoft.AspNetCore.Http.Features.IHttpBodyControlFeature>();
            featuresMock.Setup(f => f.Get<Microsoft.AspNetCore.Http.Features.IHttpBodyControlFeature>()).Returns(bodyControlFeatureMock.Object);
            httpContextMock.Setup(c => c.Features).Returns(featuresMock.Object);

            var responseMock = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
            var headers = new Microsoft.AspNetCore.Http.HeaderDictionary();
            responseMock.Setup(r => r.Headers).Returns(headers);
            responseMock.SetupProperty(r => r.ContentType);
            var stream = new MemoryStream();
            responseMock.Setup(r => r.Body).Returns(stream);

            httpContextMock.Setup(c => c.Response).Returns(responseMock.Object);
            sut.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            // Act
            await sut.DownloadLoteDeclaracoes(ids, cancellationToken);

            // Assert
            mockCasoDeUso.Verify(c => c.ExecutarAsync(ids, stream, cancellationToken), Times.Once);
            bodyControlFeatureMock.VerifySet(f => f.AllowSynchronousIO = true, Times.Once);
            responseMock.Object.ContentType.Should().Be("application/zip");
            headers.ContainsKey("Content-Disposition").Should().BeTrue();
        }
    }
}
