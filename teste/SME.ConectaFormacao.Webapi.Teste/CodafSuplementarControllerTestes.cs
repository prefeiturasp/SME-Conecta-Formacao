using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementar;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementar;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafSuplementarControllerTestes
    {
        [Fact]
        public async Task DadoCodafIdValido_QuandoChamarObterPorCodafIdAsync_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var codafId = 1;
            var casoDeUsoMock = new Mock<ICasoDeUsoObterCodafSuplementarPorCodafId>();
            var codafSuplementarDto = new CodafSuplementarDetalhadoDto
            {
                Id = codafId,
                PropostaId = 1,
                PropostaTurmaId = 1,
                DataPublicacao = DateTime.Now,
                DataPublicacaoDom = DateTime.Now,
                NumeroComunicado = 123,
                PaginaComunicadoDom = 12,
                CodigoCursoEol = 1,
                CodigoNivel = 2,
                Observacao = "Observação teste"
            };
            casoDeUsoMock.Setup(c => c.ExecutarAsync(codafId)).ReturnsAsync(codafSuplementarDto);
            var controller = new CodafSuplementarController();
            // Act
            var resultado = await controller.ObterPorCodafIdAsync(codafId, casoDeUsoMock.Object);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var resultadoValor = Assert.IsType<CodafSuplementarDetalhadoDto>(okResult.Value);
            Assert.Equal(codafSuplementarDto.Id, resultadoValor.Id);
        }
    }
}
