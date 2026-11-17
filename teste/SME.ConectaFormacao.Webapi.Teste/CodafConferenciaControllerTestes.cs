using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafConferenciaControllerTestes
    {
        private readonly Mock<ICasoDeUsoCriarCodafConferencia> _casoDeUsoCriarMock;
        private readonly Mock<ICasoDeUsoAtualizarCodafConferencia> _casoDeUsoAtualizarMock;
        private readonly Mock<ICasoDeUsoExcluirCodafConferencia> _casoDeUsoExcluirMock;
        private readonly Mock<ICasoDeUsoListarCodafConferencia> _casoDeUsoListarMock;
        private readonly Mock<ICasoDeUsoObterCodafConferenciaPorId> _casoDeUsoObterPorIdMock;
        private readonly Mock<ICasoDeUsoListarInscritosTurmaCodafConferencia> _casoDeUsoListarInscritosTurmaMock;
        private readonly Mock<ICasoDeUsoTurmaPossuiCodafConferencia> _casoDeUsoTurmaPossuiCodafMock;
        private readonly Mock<ICasoDeUsoUploadAnexoTemporarioCodafConferencia> _casoDeUsoUploadMock;
        private readonly CodafConferenciaController _sut;
        private readonly Faker _faker;

        public CodafConferenciaControllerTestes()
        {
            var mocker = new AutoMocker();
            _casoDeUsoCriarMock = mocker.GetMock<ICasoDeUsoCriarCodafConferencia>();
            _casoDeUsoAtualizarMock = mocker.GetMock<ICasoDeUsoAtualizarCodafConferencia>();
            _casoDeUsoExcluirMock = mocker.GetMock<ICasoDeUsoExcluirCodafConferencia>();
            _casoDeUsoListarMock = mocker.GetMock<ICasoDeUsoListarCodafConferencia>();
            _casoDeUsoObterPorIdMock = mocker.GetMock<ICasoDeUsoObterCodafConferenciaPorId>();
            _casoDeUsoListarInscritosTurmaMock = mocker.GetMock<ICasoDeUsoListarInscritosTurmaCodafConferencia>();
            _casoDeUsoTurmaPossuiCodafMock = mocker.GetMock<ICasoDeUsoTurmaPossuiCodafConferencia>();
            _casoDeUsoUploadMock = mocker.GetMock<ICasoDeUsoUploadAnexoTemporarioCodafConferencia>();

            _sut = mocker.CreateInstance<CodafConferenciaController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoCadastrar_EntaoDeveRetornarCreated()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto();
            var resultadoSucesso = Resultado<CodafConferenciaDetalhadoDto>.DeSucesso(new CodafConferenciaDetalhadoDto { Id = 1 });
            _casoDeUsoCriarMock.Setup(c => c.ExecutarAsync(dto)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.Cadastrar(dto, _casoDeUsoCriarMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<CreatedResult>().Subject;
            objectResult.StatusCode.Should().Be(201);
            _casoDeUsoCriarMock.Verify(c => c.ExecutarAsync(dto), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoAtualizar_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var dto = new CodafConferenciaCadastroDto();
            var resultadoSucesso = Resultado.DeSucesso();
            _casoDeUsoAtualizarMock.Setup(c => c.ExecutarAsync(dto, id)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.Atualizar(id, dto, _casoDeUsoAtualizarMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<NoContentResult>().Subject;
            objectResult.StatusCode.Should().Be(204);
            _casoDeUsoAtualizarMock.Verify(c => c.ExecutarAsync(dto, id), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoExcluir_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var resultadoSucesso = Resultado.DeSucesso();
            _casoDeUsoExcluirMock.Setup(c => c.ExecutarAsync(id)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.Excluir(id, _casoDeUsoExcluirMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<NoContentResult>().Subject;
            objectResult.StatusCode.Should().Be(204);
            _casoDeUsoExcluirMock.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterListaPaginada_EntaoDeveRetornarOk()
        {
            // Arrange
            var filtro = new FiltroCodafConferenciaDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var paginacaoDto = new PaginacaoResultadoDto<CodafConferenciaResumoDto>([], 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<CodafConferenciaResumoDto>>.DeSucesso(paginacaoDto);
            _casoDeUsoListarMock.Setup(c => c.ExecutarAsync(filtro)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ObterListaPaginada(filtro, _casoDeUsoListarMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoListarMock.Verify(c => c.ExecutarAsync(filtro), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterPorId_EntaoDeveRetornarOk()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var detalhadoDto = new CodafConferenciaDetalhadoDto { Id = id };
            var resultadoSucesso = Resultado<CodafConferenciaDetalhadoDto>.DeSucesso(detalhadoDto);
            _casoDeUsoObterPorIdMock.Setup(c => c.ExecutarAsync(id)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ObterPorId(id, _casoDeUsoObterPorIdMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoObterPorIdMock.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaTurmaIdValido_QuandoObterInscritosPorTurma_EntaoDeveRetornarOk()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 100);
            var paginacaoDto = new PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>([], 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<CodafConferenciaInscritoTurmaDto>>.DeSucesso(paginacaoDto);
            _casoDeUsoListarInscritosTurmaMock
                .Setup(c => c.ExecutarAsync(propostaTurmaId, 1, 10, null))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ObterInscritosPorTurma(propostaTurmaId, _casoDeUsoListarInscritosTurmaMock.Object, 1, 10, null);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoListarInscritosTurmaMock.Verify(c => c.ExecutarAsync(propostaTurmaId, 1, 10, null), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaTurmaId_QuandoVerificarTurmaPossuiCodaf_EntaoDeveRetornarOk()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 100);
            var resultadoSucesso = Resultado<bool>.DeSucesso(true);
            _casoDeUsoTurmaPossuiCodafMock
                .Setup(c => c.ExecutarAsync(propostaTurmaId, null))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.TurmaPossuiCodaf(propostaTurmaId, _casoDeUsoTurmaPossuiCodafMock.Object, null);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoTurmaPossuiCodafMock.Verify(c => c.ExecutarAsync(propostaTurmaId, null), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoPdfValido_QuandoUploadAnexoTemporario_EntaoDeveRetornarOk()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            var dto = new CodafAnexoTemporarioDto
            {
                ArquivoCodigo = Guid.NewGuid(),
                NomeArquivo = "ata.pdf",
                Extensao = ".pdf",
                UrlDownload = "https://url",
                ContentType = "application/pdf"
            };
            _casoDeUsoUploadMock
                .Setup(c => c.ExecutarAsync(arquivoMock.Object))
                .ReturnsAsync(Resultado<CodafAnexoTemporarioDto>.DeSucesso(dto));

            // Act
            var resultado = await _sut.UploadAnexoTemporario(arquivoMock.Object, _casoDeUsoUploadMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoUploadMock.Verify(c => c.ExecutarAsync(arquivoMock.Object), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoInvalido_QuandoUploadAnexoTemporario_EntaoDeveRetornarBadRequest()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            _casoDeUsoUploadMock
                .Setup(c => c.ExecutarAsync(arquivoMock.Object))
                .ReturnsAsync(Erro.Validacao("Extensão de arquivo '.png' não é permitida. Extensões permitidas: .pdf"));

            // Act
            var resultado = await _sut.UploadAnexoTemporario(arquivoMock.Object, _casoDeUsoUploadMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<BadRequestObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(400);
        }
    }
}
