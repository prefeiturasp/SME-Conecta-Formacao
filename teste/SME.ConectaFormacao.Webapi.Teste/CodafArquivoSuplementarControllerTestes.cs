using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafArquivoSuplementarControllerTestes
    {
        private readonly Mock<ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar> casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock;
        private readonly CodafArquivoSuplementarController sut;

        public CodafArquivoSuplementarControllerTestes()
        {
            var mocker = new AutoMocker();

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock = mocker.GetMock<ICasoDeUsoGerarArquivoRemessaConclusaoCodafSuplementar>();

            sut = mocker.CreateInstance<CodafArquivoSuplementarController>();
        }

        [Fact]
        public async Task DadoIdValidoEResultadoSucesso_QuandoGerarArquivoRemessaConclusaoCodaf_EntaoRetornaFileStreamResultComDadosDoArquivo()
        {
            // Arrange
            var codafSuplementarId = new Faker().Random.Long(1, 1000);
            var stream = new MemoryStream();
            var nomeArquivo = "remessa.csv";
            var contentType = "text/csv";
            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, stream);
            var resultadoSucesso = Resultado<ArquivoDto>.DeSucesso(arquivoDto);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock
                .Setup(c => c.ExecutarAsync(codafSuplementarId))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await sut.GerarArquivoRemessaConclusaoCodaf(codafSuplementarId);

            // Assert
            var fileResult = resultado.Should().BeOfType<FileStreamResult>().Subject;
            fileResult.FileStream.Should().BeSameAs(stream);
            fileResult.ContentType.Should().Be(contentType);
            fileResult.FileDownloadName.Should().Be(nomeArquivo);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock.Verify(c => c.ExecutarAsync(codafSuplementarId), Times.Once);
        }

        [Fact]
        public async Task DadoResultadoComFalhaNaoEncontrado_QuandoGerarArquivoRemessaConclusaoCodaf_EntaoRetornaNotFound()
        {
            // Arrange
            var codafSuplementarId = new Faker().Random.Long(1, 1000);
            var mensagemErro = "Registro não encontrado";
            var resultadoFalha = Resultado<ArquivoDto>.DeFalha(TipoFalha.NaoEncontrado, mensagemErro);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock
                .Setup(c => c.ExecutarAsync(codafSuplementarId))
                .ReturnsAsync(resultadoFalha);

            // Act
            var resultado = await sut.GerarArquivoRemessaConclusaoCodaf(codafSuplementarId);

            // Assert
            // *Alterado para o 422 pois está sendo omitido o corpo da resposta com o 404, e o front não consegue ler a mensagem de erro (by Diego Moreno - 2026-08-21)
            var notFoundResult = resultado.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            notFoundResult.StatusCode.Should().Be(422);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock.Verify(c => c.ExecutarAsync(codafSuplementarId), Times.Once);
        }

        [Fact]
        public async Task DadoResultadoComFalhaDeValidacao_QuandoGerarArquivoRemessaConclusaoCodaf_EntaoRetornaBadRequest()
        {
            // Arrange
            var codafSuplementarId = new Faker().Random.Long(1, 1000);
            var mensagemErro = "Erro de validação";
            var resultadoFalha = Resultado<ArquivoDto>.DeFalha(TipoFalha.Validacao, mensagemErro);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock
                .Setup(c => c.ExecutarAsync(codafSuplementarId))
                .ReturnsAsync(resultadoFalha);

            // Act
            var resultado = await sut.GerarArquivoRemessaConclusaoCodaf(codafSuplementarId);

            // Assert
            var badRequestResult = resultado.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.StatusCode.Should().Be(400);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock.Verify(c => c.ExecutarAsync(codafSuplementarId), Times.Once);
        }

        [Fact]
        public async Task DadoResultadoComFalhaDeRegraDeNegocio_QuandoGerarArquivoRemessaConclusaoCodaf_EntaoRetornaUnprocessableEntity()
        {
            // Arrange
            var codafSuplementarId = new Faker().Random.Long(1, 1000);
            var mensagemErro = "Erro de regra de negócio";
            var resultadoFalha = Resultado<ArquivoDto>.DeFalha(TipoFalha.RegraDeNegocio, mensagemErro);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock
                .Setup(c => c.ExecutarAsync(codafSuplementarId))
                .ReturnsAsync(resultadoFalha);

            // Act
            var resultado = await sut.GerarArquivoRemessaConclusaoCodaf(codafSuplementarId);

            // Assert
            var unprocessableEntityResult = resultado.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            unprocessableEntityResult.StatusCode.Should().Be(422);

            casoDeUsoGerarArquivoRemessaConclusaoCodafSuplementarMock.Verify(c => c.ExecutarAsync(codafSuplementarId), Times.Once);
        }
    }
}
