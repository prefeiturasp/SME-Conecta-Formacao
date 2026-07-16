using Microsoft.AspNetCore.Http;
using Moq;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Utilitarios
{
    public class ProcessadorUploadAnexoTemporarioTestes
    {
        private readonly Mock<IServicoArmazenamento> servicoArmazenamentoMock;
        private readonly ProcessadorUploadAnexoTemporario processador;

        public ProcessadorUploadAnexoTemporarioTestes()
        {
            servicoArmazenamentoMock = new Mock<IServicoArmazenamento>();

            processador = new ProcessadorUploadAnexoTemporario(
                servicoArmazenamentoMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_arquivo_for_invalido()
        {
            // Arrange
            var arquivo = new Mock<IFormFile>();

            arquivo.Setup(a => a.FileName)
                .Returns("arquivo.txt");

            arquivo.Setup(a => a.ContentType)
                .Returns("text/plain");

            arquivo.Setup(a => a.Length)
                .Returns(100);

            // Act
            var resultado = await processador.ProcessarUploadAsync(arquivo.Object);

            // Assert
            Assert.False(resultado.Sucesso);

            servicoArmazenamentoMock.Verify(
                x => x.ArmazenarTemporariaGuid(
                    It.IsAny<Stream>(),
                    It.IsAny<string>()),
                Times.Never);

            servicoArmazenamentoMock.Verify(
                x => x.ObterUrlPorChaveObjetoAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_guid_retornado_for_zerado()
        {
            var bytes = Encoding.UTF8.GetBytes("PDF fake");
            var stream = new MemoryStream(bytes);

            var arquivo = new Mock<IFormFile>();

            arquivo.Setup(a => a.FileName)
                .Returns("arquivo.pdf");

            arquivo.Setup(a => a.ContentType)
                .Returns("application/pdf");

            arquivo.Setup(a => a.Length)
                .Returns(bytes.Length);

            arquivo.Setup(a => a.OpenReadStream())
                .Returns(stream);

            servicoArmazenamentoMock
                .Setup(x => x.ArmazenarTemporariaGuid(
                    It.IsAny<Stream>(),
                    "application/pdf"))
                .ReturnsAsync(Guid.Empty);

            var resultado = await processador.ProcessarUploadAsync(arquivo.Object);

            Assert.False(resultado.Sucesso);
            Assert.NotNull(resultado.MensagensErro);
            Assert.Single(resultado.MensagensErro);
            Assert.Equal("Não foi possível salvar o anexo. Tente novamente.", resultado.MensagensErro?.First());

            servicoArmazenamentoMock.Verify(
                x => x.ObterUrlPorChaveObjetoAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_processar_upload_com_sucesso()
        {
            // Arrange
            var arquivoId = Guid.NewGuid();
            const string url = "https://teste.com/download";

            var bytes = Encoding.UTF8.GetBytes("PDF fake");
            var stream = new MemoryStream(bytes);

            var arquivo = new Mock<IFormFile>();

            arquivo.Setup(a => a.FileName)
                .Returns("arquivo.pdf");

            arquivo.Setup(a => a.ContentType)
                .Returns("application/pdf");

            arquivo.Setup(a => a.Length)
                .Returns(bytes.Length);

            arquivo.Setup(a => a.OpenReadStream())
                .Returns(stream);

            servicoArmazenamentoMock
                .Setup(x => x.ArmazenarTemporariaGuid(
                    It.IsAny<Stream>(),
                    "application/pdf"))
                .ReturnsAsync(arquivoId);

            servicoArmazenamentoMock
                .Setup(x => x.ObterUrlPorChaveObjetoAsync(
                    arquivoId.ToString(),
                    true))
                .ReturnsAsync(url);

            // Act
            var resultado = await processador.ProcessarUploadAsync(arquivo.Object);

            // Assert
            Assert.True(resultado.Sucesso);

            Assert.NotNull(resultado.Dados);
            Assert.Equal(arquivoId, resultado.Dados.ArquivoCodigo);
            Assert.Equal("arquivo.pdf", resultado.Dados.NomeArquivo);
            Assert.Equal(".pdf", resultado.Dados.Extensao);
            Assert.Equal(url, resultado.Dados.UrlDownload);
            Assert.Equal("application/pdf", resultado.Dados.ContentType);
            Assert.Equal(bytes.Length, resultado.Dados.TamanhoBytes);

            servicoArmazenamentoMock.Verify(
                x => x.ArmazenarTemporariaGuid(
                    It.IsAny<Stream>(),
                    "application/pdf"),
                Times.Once);

            servicoArmazenamentoMock.Verify(
                x => x.ObterUrlPorChaveObjetoAsync(
                    arquivoId.ToString(),
                    true),
                Times.Once);
        }
    }
}
