using Microsoft.AspNetCore.Http;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoUploadAnexoTemporarioCodafSuplementarTestes
    {
        private readonly Mock<IServicoArmazenamento> servicoArmazenamentoMock;
        private readonly CasoDeUsoUploadAnexoTemporarioCodafSuplementar casoDeUso;

        public CasoDeUsoUploadAnexoTemporarioCodafSuplementarTestes()
        {
            servicoArmazenamentoMock = new Mock<IServicoArmazenamento>();

            casoDeUso = new CasoDeUsoUploadAnexoTemporarioCodafSuplementar(
                servicoArmazenamentoMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_arquivo_for_invalido()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>();

            arquivo.Setup(x => x.FileName).Returns("arquivo.txt");
            arquivo.Setup(x => x.ContentType).Returns("text/plain");
            arquivo.Setup(x => x.Length).Returns(100);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(arquivo.Object);

            // Assert
            Assert.False(resultado.Sucesso);

            servicoArmazenamentoMock.Verify(x =>
                x.ArmazenarTemporariaGuid(It.IsAny<Stream>(), It.IsAny<string>()),
                Times.Never);

            servicoArmazenamentoMock.Verify(x =>
                x.ObterUrlPorChaveObjetoAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_realizar_upload_e_retornar_dados_do_anexo()
        {
            // Arrange
            var arquivoId = Guid.NewGuid();
            const string url = "https://teste.com/arquivo.pdf";

            var conteudo = Encoding.UTF8.GetBytes("conteudo pdf");
            var stream = new MemoryStream(conteudo);

            var arquivo = new Mock<IFormFile>();

            arquivo.Setup(x => x.FileName).Returns("arquivo.pdf");
            arquivo.Setup(x => x.ContentType).Returns("application/pdf");
            arquivo.Setup(x => x.Length).Returns(conteudo.Length);
            arquivo.Setup(x => x.OpenReadStream()).Returns(stream);

            servicoArmazenamentoMock
                .Setup(x => x.ArmazenarTemporariaGuid(It.IsAny<Stream>(), "application/pdf"))
                .ReturnsAsync(arquivoId);

            servicoArmazenamentoMock
                .Setup(x => x.ObterUrlPorChaveObjetoAsync(arquivoId.ToString(), true))
                .ReturnsAsync(url);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(arquivo.Object);

            // Assert
            Assert.True(resultado.Sucesso);

            var dto = resultado.Dados;

            Assert.NotNull(dto);
            Assert.Equal(arquivoId, dto.ArquivoCodigo);
            Assert.Equal("arquivo.pdf", dto.NomeArquivo);
            Assert.Equal(".pdf", dto.Extensao);
            Assert.Equal(url, dto.UrlDownload);
            Assert.Equal("application/pdf", dto.ContentType);
            Assert.Equal(conteudo.Length, dto.TamanhoBytes);

            servicoArmazenamentoMock.Verify(x =>
                x.ArmazenarTemporariaGuid(It.IsAny<Stream>(), "application/pdf"),
                Times.Once);

            servicoArmazenamentoMock.Verify(x =>
                x.ObterUrlPorChaveObjetoAsync(arquivoId.ToString(), true),
                Times.Once);
        }
    }
}
