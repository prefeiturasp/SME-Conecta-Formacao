using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Opcoes;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Armazenamento
{
    public class ServicoArmazenamentoTests
    {
        private readonly Mock<IMinioClient> _minioClientMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ServicoArmazenamento _servico;
        private readonly ConfiguracaoArmazenamentoOptions _options;

        public ServicoArmazenamentoTests()
        {
            var mocker = new AutoMocker();
            _minioClientMock = mocker.GetMock<IMinioClient>();
            _configurationMock = mocker.GetMock<IConfiguration>();

            _options = new ConfiguracaoArmazenamentoOptions
            {
                BucketTemp = "temp-bucket",
                BucketArquivos = "files-bucket",
                EndPoint = "play.min.io",
                AccessKey = "teste",
                SecretKey = "teste"
            };

            var optionsMock = mocker.GetMock<IOptions<ConfiguracaoArmazenamentoOptions>>();
            optionsMock.Setup(x => x.Value).Returns(_options);
            mocker.Use(optionsMock.Object);

            // Configuração fake para URL legado
            _configurationMock.Setup(x => x["UrlBucket"]).Returns("http://localhost");

            _servico = mocker.CreateInstance<ServicoArmazenamento>();
        }

        [Fact]
        public async Task ArmazenarTemporariaGuid_Deve_Chamar_PutObjectAsync_E_Retornar_Guid()
        {
            // Arrange
            var stream = new MemoryStream([1, 2, 3]);
            var contentType = "application/pdf";

            // Act
            var guid = await _servico.ArmazenarTemporariaGuid(stream, contentType);

            // Assert
            Assert.NotEqual(Guid.Empty, guid);

            _minioClientMock.Verify(x => x.PutObjectAsync(
                It.Is<PutObjectArgs>(args =>
                    args != null &&
                    true
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MoverGuid_Deve_Chamar_Copy_E_Remove_Se_Buckets_Diferentes()
        {
            // Arrange
            var guid = Guid.NewGuid();

            _minioClientMock.Setup(x => x.CopyObjectAsync(It.IsAny<CopyObjectArgs>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _minioClientMock.Setup(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _servico.MoverGuid(guid);

            // Assert
            Assert.Equal(guid, resultado);
            _minioClientMock.Verify(x => x.CopyObjectAsync(It.IsAny<CopyObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
            _minioClientMock.Verify(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterUrlPorGuidAsync_Deve_Retornar_Url_Assinada()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var urlEsperada = "https://minio.com/bucket/arquivo?token=xyz";

            _minioClientMock
                .Setup(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
                .ReturnsAsync(urlEsperada);

            // Act
            var urlRetornada = await _servico.ObterUrlPorChaveObjetoAsync(guid.ToString());

            // Assert
            Assert.Equal(urlEsperada, urlRetornada);
            _minioClientMock.Verify(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()), Times.Once);
        }
    }
}