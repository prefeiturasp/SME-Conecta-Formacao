using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafArquivoControllerTests
    {
        private readonly Mock<ICasoDeUsoObterModeloTermoResponsabilidadeCodaf> _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf;
        private readonly Mock<ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca> _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca;
        private readonly Mock<ICasoDeUsoGerarArquivoRemessaConclusaoCodaf> _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol;
        private readonly CodafArquivoController _controller;

        public CodafArquivoControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf = mocker.GetMock<ICasoDeUsoObterModeloTermoResponsabilidadeCodaf>();
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca = mocker.GetMock<ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca>();
            _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol = mocker.GetMock<ICasoDeUsoGerarArquivoRemessaConclusaoCodaf>();
            _controller = mocker.CreateInstance<CodafArquivoController>();
        }


        [Fact]
        public async Task DadoSolicitacaoDeModelo_QuandoArquivoExistir_EntaoDeveRetornarFileStreamResult()
        {
            // Arrange
            var nomeArquivo = "TermoResponsabilidadeModelo.pdf";
            var contentType = "application/pdf";
            var memoryStream = new MemoryStream([1, 2, 3]);

            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, memoryStream);
            var resultadoSucesso = Resultado<ArquivoDto>.DeSucesso(arquivoDto);

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Setup(x => x.Executar())
                .Returns(resultadoSucesso);

            // Act
            var resultado = await _controller.ObterModeloTermoResponsabilidade();

            // Assert
            var fileResult = resultado.Should().BeOfType<FileStreamResult>().Subject;

            fileResult.ContentType.Should().Be(contentType);
            fileResult.FileDownloadName.Should().Be(nomeArquivo);
            fileResult.FileStream.Should().BeSameAs(memoryStream);

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Verify(x => x.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoSolicitacaoDeModelo_QuandoArquivoNaoForEncontrado_EntaoDeveRetornarNotFound()
        {
            // Arrange
            var erro = Erro.NaoEncontrado("Modelo não encontrado.");

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Setup(x => x.Executar())
                .Returns(erro);

            // Act
            var resultado = await _controller.ObterModeloTermoResponsabilidade();

            // Assert
            // *Alterado para o 422 pois está sendo omitido o corpo da resposta com o 404, e o front não consegue ler a mensagem de erro (by Diego Moreno - 2026-08-21)
            var notFoundResult = resultado.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(422);

            var valorRetorno = notFoundResult.Value;
            valorRetorno.Should().NotBeNull();
        }

        [Fact]
        public async Task DadoUmArquivoValido_QuandoChamarUploadAnexoTemporario_EntaoDeveChamarCasoDeUsoUploadAnexoTemporarioCodafListaPresenca()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(1024); // 1 KB
            arquivoMock.Setup(a => a.FileName).Returns("documento.pdf");
            arquivoMock.Setup(a => a.ContentType).Returns("application/pdf");
            arquivoMock.Setup(a => a.OpenReadStream()).Returns(new MemoryStream([1, 2, 3]));
            var arquivoDto = arquivoMock.Object;
            var arquivoTemporarioDto = new CodafAnexoTemporarioDto { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = "documento.pdf", ContentType = "application/pdf", TamanhoBytes = 1024 };
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca
                .Setup(x => x.ExecutarAsync(arquivoDto))
                .ReturnsAsync(Resultado<CodafAnexoTemporarioDto>.DeSucesso(arquivoTemporarioDto));
            // Act
            await _controller.UploadAnexoTemporario(arquivoDto);
            // Assert
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca.Verify(x => x.ExecutarAsync(arquivoDto), Times.Once);
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaIdValido_QuandoGerarArquivoDeInscricoesParaEol_EntaoDeveRetornarFileStreamResult()
        {
            // Arrange
            long codafListaPresencaId = 1;
            var nomeArquivo = "HOM2023001TurmaAAvancada.txt";
            var contentType = "application/octet-stream";
            var memoryStream = new MemoryStream([1, 2, 3]);
            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, memoryStream);
            var resultadoSucesso = Resultado<ArquivoDto>.DeSucesso(arquivoDto);
            _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(resultadoSucesso);
            // Act
            var resultado = await _controller.GerarArquivoRemessaConclusaoCodaf(codafListaPresencaId);
            // Assert
            var fileResult = resultado.Should().BeOfType<FileStreamResult>().Subject;
            fileResult.ContentType.Should().Be(contentType);
            fileResult.FileDownloadName.Should().Be(nomeArquivo);
            fileResult.FileStream.Should().BeSameAs(memoryStream);
            _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
                .Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaIdInvalido_QuandoGerarArquivoDeInscricoesParaEol_EntaoDeveRetornarNotFound()
        {
            // Arrange
            long codafListaPresencaId = 1;
            var erro = Erro.NaoEncontrado("Dados não encontrados para o CodafId informado.");
            _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(erro);
            // Act
            var resultado = await _controller.GerarArquivoRemessaConclusaoCodaf(codafListaPresencaId);

            // Assert
            // *Alterado para o 422 pois está sendo omitido o corpo da resposta com o 404, e o front não consegue ler a mensagem de erro (by Diego Moreno - 2026-08-21)
            var notFoundResult = resultado.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;

            notFoundResult.StatusCode.Should().Be(422);
            var valorRetorno = notFoundResult.Value;
            valorRetorno.Should().NotBeNull();
            _mockCasoDeUsoGerarArquivoDeInscricoesDoCodafParaEol
                .Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }
    }
}
