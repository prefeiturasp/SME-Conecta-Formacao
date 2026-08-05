using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste.Controllers
{
    public class CodafCursoNaoHomologadoControllerTestes
    {
        private readonly Mock<ICasoDeUsoCriarCodafCursoNaoHomologado> _casoDeUsoCriarMock;
        private readonly Mock<ICasoDeUsoAtualizarCodafCursoNaoHomologado> _casoDeUsoAtualizarMock;
        private readonly Mock<ICasoDeUsoExcluirCodafCursoNaoHomologado> _casoDeUsoExcluirMock;
        private readonly Mock<ICasoDeUsoListarCodafCursoNaoHomologado> _casoDeUsoListarMock;
        private readonly Mock<ICasoDeUsoObterCodafCursoNaoHomologadoPorId> _casoDeUsoObterPorIdMock;
        private readonly Mock<ICasoDeUsoUploadAnexoTemporarioCodafCursoNaoHomologado> _casoDeUsoUploadMock;
        private readonly CodafCursoNaoHomologadoController _sut;
        private readonly Faker _faker;

        public CodafCursoNaoHomologadoControllerTestes()
        {
            var mocker = new AutoMocker();
            _casoDeUsoCriarMock = mocker.GetMock<ICasoDeUsoCriarCodafCursoNaoHomologado>();
            _casoDeUsoAtualizarMock = mocker.GetMock<ICasoDeUsoAtualizarCodafCursoNaoHomologado>();
            _casoDeUsoExcluirMock = mocker.GetMock<ICasoDeUsoExcluirCodafCursoNaoHomologado>();
            _casoDeUsoListarMock = mocker.GetMock<ICasoDeUsoListarCodafCursoNaoHomologado>();
            _casoDeUsoObterPorIdMock = mocker.GetMock<ICasoDeUsoObterCodafCursoNaoHomologadoPorId>();
            _casoDeUsoUploadMock = mocker.GetMock<ICasoDeUsoUploadAnexoTemporarioCodafCursoNaoHomologado>();

            _sut = mocker.CreateInstance<CodafCursoNaoHomologadoController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoCadastrar_EntaoDeveRetornarCreated()
        {
            // Arrange
            var dto = new CodafCursoNaoHomologadoCadastroDto();
            var detalhadoDto = new CodafCursoNaoHomologadoDetalhadoDto { Id = _faker.Random.Long(1, 100) };
            var resultadoSucesso = Resultado<CodafCursoNaoHomologadoDetalhadoDto>.DeSucesso(detalhadoDto);

            _casoDeUsoCriarMock.Setup(c => c.ExecutarAsync(dto)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.Cadastrar(dto, _casoDeUsoCriarMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(201);
            _casoDeUsoCriarMock.Verify(c => c.ExecutarAsync(dto), Times.Once);
        }

        [Fact]
        public async Task DadoDtoValido_QuandoAtualizar_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var id = _faker.Random.Int(1, 100);
            var dto = new CodafCursoNaoHomologadoCadastroDto();
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
            var filtro = new FiltroCodafCursoNaoHomologadoDto 
            { 
                NumeroPagina = 1, 
                NumeroRegistros = 10 
            };
            var paginacaoDto = new PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>(new List<CodafCursoNaoHomologadoResumoDto>(), 0, 10);
            var resultadoSucesso = Resultado<PaginacaoResultadoDto<CodafCursoNaoHomologadoResumoDto>>.DeSucesso(paginacaoDto);

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
            var detalhadoDto = new CodafCursoNaoHomologadoDetalhadoDto();
            var resultadoSucesso = Resultado<CodafCursoNaoHomologadoDetalhadoDto>.DeSucesso(detalhadoDto);

            _casoDeUsoObterPorIdMock.Setup(c => c.ExecutarAsync(id)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.ObterPorId(id, _casoDeUsoObterPorIdMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(200);
            _casoDeUsoObterPorIdMock.Verify(c => c.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoArquivoValido_QuandoUploadAnexoTemporario_EntaoDeveRetornarCreated()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            var anexoTemporarioDto = new CodafAnexoTemporarioDto();
            var resultadoSucesso = Resultado<CodafAnexoTemporarioDto>.DeSucesso(anexoTemporarioDto);

            _casoDeUsoUploadMock.Setup(c => c.ExecutarAsync(arquivoMock.Object)).ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _sut.UploadAnexoTemporario(arquivoMock.Object, _casoDeUsoUploadMock.Object);

            // Assert
            var objectResult = resultado.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(201);
            _casoDeUsoUploadMock.Verify(c => c.ExecutarAsync(arquivoMock.Object), Times.Once);
        }
    }
}
