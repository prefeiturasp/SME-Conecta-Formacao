using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ImportacaoArquivoControllerTestes
    {
        private readonly Mock<ICasoDeUsoImportacaoArquivoInscricaoCursista> _mockImportacao;
        private readonly Mock<ICasoDeUsoObterArquivosInscricaoImportados> _mockObterArquivos;
        private readonly Mock<ICasoDeUsoObterRegistrosDaIncricaoInconsistentes> _mockObterInconsistentes;
        private readonly Mock<ICasoDeUsoInscricaoManualContinuarProcessamento> _mockContinuar;
        private readonly Mock<ICasoDeUsoInscricaoManualCancelarProcessamento> _mockCancelar;
        private readonly ImportacaoArquivoController _sut;

        public ImportacaoArquivoControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockImportacao = mocker.GetMock<ICasoDeUsoImportacaoArquivoInscricaoCursista>();
            _mockObterArquivos = mocker.GetMock<ICasoDeUsoObterArquivosInscricaoImportados>();
            _mockObterInconsistentes = mocker.GetMock<ICasoDeUsoObterRegistrosDaIncricaoInconsistentes>();
            _mockContinuar = mocker.GetMock<ICasoDeUsoInscricaoManualContinuarProcessamento>();
            _mockCancelar = mocker.GetMock<ICasoDeUsoInscricaoManualCancelarProcessamento>();
            _sut = mocker.CreateInstance<ImportacaoArquivoController>();
        }

        [Fact]
        public async Task DadoArquivoEPropostaValidos_QuandoImportarArquivoInscricaoCursista_EntaoRetornaOk()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>().Object;
            var propostaId = 1L;
            var retorno = new RetornoDTO();
            _mockImportacao.Setup(m => m.Executar(arquivo, propostaId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ImportarArquivoInscricaoCursista(arquivo, propostaId, _mockImportacao.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockImportacao.Verify(m => m.Executar(arquivo, propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoObterArquivosImportados_EntaoRetornaPaginacao()
        {
            // Arrange
            var propostaId = 1L;
            var retorno = new PaginacaoResultadoDto<ArquivoInscricaoImportadoDTO>(new List<ArquivoInscricaoImportadoDTO>(), 0, 0);
            _mockObterArquivos.Setup(m => m.Executar(propostaId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterArquivosImportados(propostaId, _mockObterArquivos.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterArquivos.Verify(m => m.Executar(propostaId), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoIdValido_QuandoObterRegistrosComInconsistencia_EntaoRetornaPaginacao()
        {
            // Arrange
            var arquivoId = 1L;
            var retorno = new PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>(new List<RegistroDaInscricaoInsconsistenteDto>(), 0, 0, false);
            _mockObterInconsistentes.Setup(m => m.Executar(arquivoId)).ReturnsAsync(retorno);

            // Act
            var resultado = await _sut.ObterRegistrosComInconsistencia(arquivoId, _mockObterInconsistentes.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retorno);
            _mockObterInconsistentes.Verify(m => m.Executar(arquivoId), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoIdValido_QuandoContinuarProcessamentoArquivo_EntaoRetornaTrue()
        {
            // Arrange
            var arquivoId = 1L;
            _mockContinuar.Setup(m => m.Executar(arquivoId)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.ContinuarProcessamentoArquivo(arquivoId, _mockContinuar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockContinuar.Verify(m => m.Executar(arquivoId), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoIdValido_QuandoCancelarProcessamentoArquivo_EntaoRetornaTrue()
        {
            // Arrange
            var arquivoId = 1L;
            _mockCancelar.Setup(m => m.Executar(arquivoId)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.CancelarProcessamentoArquivo(arquivoId, _mockCancelar.Object) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().Be(true);
            _mockCancelar.Verify(m => m.Executar(arquivoId), Times.Once);
        }
    }
}
