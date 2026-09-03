using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CodafSuplementarControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterCodafSuplementarPorCodafId> _mockObterPorCodafId;
        private readonly Mock<ICasoDeUsoCriarCodafSuplementar> _mockCriar;
        private readonly Mock<ICasoDeUsoAtualizarCodafSuplementar> _mockAtualizar;
        private readonly Mock<ICasoDeUsoExcluirCodafSuplementar> _mockExcluir;
        private readonly Mock<ICasoDeUsoListarCodafSuplementar> _mockListar;
        private readonly Mock<ICasoDeUsoObterCodafSuplementarPorId> _mockObterPorId;
        private readonly Mock<ICasoDeUsoUploadAnexoTemporarioCodafSuplementar> _mockUpload;
        private readonly Mock<ICasoDeUsoRemoverCodafSuplementarRetificacao> _mockRemoverRetificacao;
        private readonly Mock<ICasoDeUsoGerarRelatorioCodafSuplementar> _mockImprimir;
        private readonly Mock<ICasoDeUsoFinalizarCodafSuplementar> _mockFinalizar;
        private readonly CodafSuplementarController _sut;
        private readonly Faker _faker;

        public CodafSuplementarControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockObterPorCodafId = mocker.GetMock<ICasoDeUsoObterCodafSuplementarPorCodafId>();
            _mockCriar = mocker.GetMock<ICasoDeUsoCriarCodafSuplementar>();
            _mockAtualizar = mocker.GetMock<ICasoDeUsoAtualizarCodafSuplementar>();
            _mockExcluir = mocker.GetMock<ICasoDeUsoExcluirCodafSuplementar>();
            _mockListar = mocker.GetMock<ICasoDeUsoListarCodafSuplementar>();
            _mockObterPorId = mocker.GetMock<ICasoDeUsoObterCodafSuplementarPorId>();
            _mockUpload = mocker.GetMock<ICasoDeUsoUploadAnexoTemporarioCodafSuplementar>();
            _mockRemoverRetificacao = mocker.GetMock<ICasoDeUsoRemoverCodafSuplementarRetificacao>();
            _mockImprimir = mocker.GetMock<ICasoDeUsoGerarRelatorioCodafSuplementar>();
            _mockFinalizar = mocker.GetMock<ICasoDeUsoFinalizarCodafSuplementar>();
            _sut = mocker.CreateInstance<CodafSuplementarController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafIdValido_QuandoObterPorCodafId_EntaoDeveRetornarOk()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            var resultado = Resultado<CodafSuplementarDetalhadoDto>.DeSucesso(new CodafSuplementarDetalhadoDto());
            _mockObterPorCodafId.Setup(x => x.ExecutarAsync(codafId)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.ObterPorCodafIdAsync(codafId, _mockObterPorCodafId.Object) as OkObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockObterPorCodafId.Verify(x => x.ExecutarAsync(codafId), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoCadastrar_EntaoDeveRetornarCreated()
        {
            // Arrange
            var dto = new CodafSuplementarCadastroDto();
            var resultado = Resultado<CodafSuplementarDetalhadoDto>.DeSucesso(new CodafSuplementarDetalhadoDto());
            _mockCriar.Setup(x => x.ExecutarAsync(dto)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.Cadastrar(dto, _mockCriar.Object) as ObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.Created);
            _mockCriar.Verify(x => x.ExecutarAsync(dto), Times.Once);
        }

        [Fact]
        public async Task DadoDtoEIdValidos_QuandoAtualizar_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new CodafSuplementarCadastroDto();
            var resultado = Resultado.DeSucesso();
            _mockAtualizar.Setup(x => x.ExecutarAsync(dto, id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.Atualizar(id, dto, _mockAtualizar.Object) as StatusCodeResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockAtualizar.Verify(x => x.ExecutarAsync(dto, id), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoExcluir_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = 1L;
            var resultado = Resultado.DeSucesso();
            _mockExcluir.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.Excluir(id, _mockExcluir.Object) as StatusCodeResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockExcluir.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterListaPaginada_EntaoDeveRetornarOk()
        {
            // Arrange
            var filtro = new FiltroCodafSuplementarDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var paginacao = new PaginacaoResultadoDto<CodafSuplementarResumoDto>([], 0, 0);
            var resultado = Resultado<PaginacaoResultadoDto<CodafSuplementarResumoDto>>.DeSucesso(paginacao);
            _mockListar.Setup(x => x.ExecutarAsync(filtro)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.ObterListaPaginada(filtro, _mockListar.Object) as OkObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockListar.Verify(x => x.ExecutarAsync(filtro), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterPorId_EntaoDeveRetornarOk()
        {
            // Arrange
            var id = 1L;
            var resultado = Resultado<CodafSuplementarDetalhadoDto>.DeSucesso(new CodafSuplementarDetalhadoDto());
            _mockObterPorId.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.ObterPorId(id, _mockObterPorId.Object) as OkObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockObterPorId.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoValido_QuandoUploadAnexoTemporario_EntaoDeveRetornarOk()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>().Object;
            var resultado = Resultado<CodafAnexoTemporarioDto>.DeSucesso(new CodafAnexoTemporarioDto());
            _mockUpload.Setup(x => x.ExecutarAsync(arquivo)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.UploadAnexoTemporario(arquivo, _mockUpload.Object) as OkObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockUpload.Verify(x => x.ExecutarAsync(arquivo), Times.Once);
        }

        [Fact]
        public async Task DadoRetificacaoIdValido_QuandoRemoverRetificacao_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = 1L;
            var resultado = Resultado.DeSucesso();
            _mockRemoverRetificacao.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.RemoverRetificacao(id, _mockRemoverRetificacao.Object);

            // Assert
            var result = act.Should().BeOfType<OkObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            _mockRemoverRetificacao.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoCodafIdValido_QuandoImprimirRelatorioCodaf_ERetornarSucesso_EntaoDeveRetornarFileStream()
        {
            // Arrange
            var id = 1L;
            var stream = new MemoryStream([1]);
            var arquivoDto = new ArquivoDto("nome.pdf", "application/pdf", stream);
            var resultado = Resultado<ArquivoDto>.DeSucesso(arquivoDto);
            _mockImprimir.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.ImprimirRelatorioCodafAsync(id, _mockImprimir.Object) as FileStreamResult;

            // Assert
            act.Should().NotBeNull();
            act!.FileStream.Should().BeSameAs(stream);
            act.FileDownloadName.Should().Be("nome.pdf");
            act.ContentType.Should().Be("application/pdf");
            _mockImprimir.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoCodafIdValido_QuandoImprimirRelatorioCodaf_ERetornarErro_EntaoDeveRetornarBadRequest()
        {
            // Arrange
            var id = 1L;
            var erro = Erro.NaoEncontrado("Erro");
            _mockImprimir.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(erro);

            // Act
            var act = await _sut.ImprimirRelatorioCodafAsync(id, _mockImprimir.Object) as UnprocessableEntityObjectResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
            _mockImprimir.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoCodafSuplementarIdValido_QuandoFinalizarCodaf_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = 1L;
            var resultado = Resultado.DeSucesso();
            _mockFinalizar.Setup(x => x.ExecutarAsync(id)).ReturnsAsync(resultado);

            // Act
            var act = await _sut.FinalizarCodafAsync(id, _mockFinalizar.Object) as StatusCodeResult;

            // Assert
            act.Should().NotBeNull();
            act!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
            _mockFinalizar.Verify(x => x.ExecutarAsync(id), Times.Once);
        }
    }
}
