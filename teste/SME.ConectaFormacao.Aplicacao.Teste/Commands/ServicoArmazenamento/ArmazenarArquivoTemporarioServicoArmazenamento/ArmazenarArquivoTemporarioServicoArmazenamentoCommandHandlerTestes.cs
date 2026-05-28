using Bogus;
using Microsoft.AspNetCore.Http;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ServicoArmazenamento.ArmazenarArquivoTemporarioServicoArmazenamento
{
    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandlerTestes
    {
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler _handler;
        private readonly Faker _faker;

        public ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandlerTestes()
        {
            _servicoArmazenamentoMock = new Mock<IServicoArmazenamento>();
            _handler = new ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(_servicoArmazenamentoMock.Object);
            _faker = new Faker("pt_BR");
        }

        #region Testes de Sucesso

        [Fact(DisplayName = "Handle - Deve armazenar arquivo temporário com sucesso")]
        public async Task Handle_Deve_Armazenar_Arquivo_Temporario_Com_Sucesso()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("documento.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("documento.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);
            var urlEsperada = $"https://storage.example.com/temp/{codigo}.pdf";

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(urlEsperada);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(urlEsperada, resultado);
            _servicoArmazenamentoMock.Verify(
                s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact(DisplayName = "Handle - Deve construir nome do arquivo com GUID e extensão")]
        public async Task Handle_Deve_Construir_Nome_Arquivo_Com_Guid_E_Extensao()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var extensao = ".docx";
            var nomeArquivo = $"relatorio{extensao}";
            var formFileMock = CriarFormFileMock(nomeArquivo, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            var arquivoDto = new ArquivoDTO(nomeArquivo, codigo, TipoArquivo.Temp, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            string? nomeArquivoCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => nomeArquivoCapturado = nome)
                .ReturnsAsync($"https://storage.example.com/temp/{nomeArquivoCapturado}");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(nomeArquivoCapturado);
            Assert.Equal($"{codigo}{extensao}", nomeArquivoCapturado);
        }

        [Fact(DisplayName = "Handle - Deve passar stream do FormFile para armazenamento")]
        public async Task Handle_Deve_Passar_Stream_FormFile_Para_Armazenamento()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var conteudo = "Conteúdo do arquivo"u8.ToArray();
            var formFileMock = CriarFormFileMock("arquivo.txt", "text/plain", conteudo);
            var arquivoDto = new ArquivoDTO("arquivo.txt", codigo, TipoArquivo.Temp, "text/plain", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            Stream? streamCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => streamCapturado = stream)
                .ReturnsAsync("https://storage.example.com/temp/arquivo.txt");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(streamCapturado);
            Assert.IsType<Stream>(streamCapturado, exactMatch: false);
        }

        [Fact(DisplayName = "Handle - Deve passar tipo de conteúdo correto para armazenamento")]
        public async Task Handle_Deve_Passar_Tipo_Conteudo_Correto_Para_Armazenamento()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var tipoConteudoEsperado = "application/vnd.ms-excel";
            var formFileMock = CriarFormFileMock("dados.xlsx", tipoConteudoEsperado);
            var arquivoDto = new ArquivoDTO("dados.xlsx", codigo, TipoArquivo.Temp, tipoConteudoEsperado, formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            string? tipoConteudoCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => tipoConteudoCapturado = contentType)
                .ReturnsAsync("https://storage.example.com/temp/dados.xlsx");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(tipoConteudoEsperado, tipoConteudoCapturado);
        }

        [Fact(DisplayName = "Handle - Deve retornar URL do armazenamento temporário")]
        public async Task Handle_Deve_Retornar_Url_Armazenamento_Temporario()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("teste.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("teste.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);
            var urlRetornada = $"https://bucket.storage/temp/{codigo}.pdf";

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(urlRetornada);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(urlRetornada, resultado);
            Assert.NotEmpty(resultado);
        }

        #endregion

        #region Testes com Diferentes Extensões

        [Theory(DisplayName = "Handle - Deve armazenar arquivos com diferentes extensões")]
        [InlineData("documento.pdf", "application/pdf")]
        [InlineData("planilha.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [InlineData("apresentacao.pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
        [InlineData("imagem.png", "image/png")]
        [InlineData("imagem.jpg", "image/jpeg")]
        [InlineData("arquivo.txt", "text/plain")]
        [InlineData("dados.csv", "text/csv")]
        [InlineData("video.mp4", "video/mp4")]
        public async Task Handle_Deve_Armazenar_Diferentes_Extensoes(string nomeArquivo, string contentType)
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var extensaoEsperada = Path.GetExtension(nomeArquivo);
            var formFileMock = CriarFormFileMock(nomeArquivo, contentType);
            var arquivoDto = new ArquivoDTO(nomeArquivo, codigo, TipoArquivo.Temp, contentType, formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            string? nomeArquivoCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, type) => nomeArquivoCapturado = nome)
                .ReturnsAsync($"https://storage.example.com/{nomeArquivoCapturado}");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal($"{codigo}{extensaoEsperada}", nomeArquivoCapturado);
        }

        #endregion

        #region Testes com GUID Diferente

        [Fact(DisplayName = "Handle - Deve usar GUID correto do arquivo DTO")]
        public async Task Handle_Deve_Usar_Guid_Correto_Do_Arquivo_Dto()
        {
            // Arrange
            var guidEsperado = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", guidEsperado, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            string? nomeArquivoCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => nomeArquivoCapturado = nome)
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.StartsWith(guidEsperado.ToString(), nomeArquivoCapturado!);
        }

        [Theory(DisplayName = "Handle - Deve armazenar com múltiplos GUIDs diferentes")]
        [InlineData("arquivo1.pdf")]
        [InlineData("arquivo2.pdf")]
        [InlineData("arquivo3.pdf")]
        public async Task Handle_Deve_Armazenar_Com_Multiplos_Guids(string nomeArquivo)
        {
            // Arrange
            var guids = new List<Guid> 
            { 
                Guid.NewGuid(), 
                Guid.NewGuid(), 
                Guid.NewGuid() 
            };

            foreach (var guid in guids)
            {
                var formFileMock = CriarFormFileMock(nomeArquivo, "application/pdf");
                var arquivoDto = new ArquivoDTO(nomeArquivo, guid, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
                var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

                _servicoArmazenamentoMock
                    .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                    .ReturnsAsync($"https://storage.example.com/temp/{guid}.pdf");

                // Act
                var resultado = await _handler.Handle(command, CancellationToken.None);

                // Assert
                Assert.NotEmpty(resultado);
                Assert.Contains(guid.ToString(), resultado);
            }
        }

        #endregion

        #region Testes de Stream OpenReadStream

        [Fact(DisplayName = "Handle - Deve chamar OpenReadStream do FormFile")]
        public async Task Handle_Deve_Chamar_OpenReadStream_Do_FormFile()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            formFileMock.Verify(f => f.OpenReadStream(), Times.Once);
        }

        [Fact(DisplayName = "Handle - Deve usar stream retornado por OpenReadStream")]
        public async Task Handle_Deve_Usar_Stream_Retornado_Por_OpenReadStream()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var streamMock = new MemoryStream("Conteúdo do arquivo"u8.ToArray());
            var formFileMock = new Mock<IFormFile>();
            
            formFileMock.Setup(f => f.FileName).Returns("arquivo.txt");
            formFileMock.Setup(f => f.OpenReadStream()).Returns(streamMock);

            var arquivoDto = new ArquivoDTO("arquivo.txt", codigo, TipoArquivo.Temp, "text/plain", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            Stream? streamRecebido = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => streamRecebido = stream)
                .ReturnsAsync("https://storage.example.com/arquivo.txt");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(streamRecebido);
            Assert.Same(streamMock, streamRecebido);
        }

        #endregion

        #region Testes de Path.GetExtension

        [Theory(DisplayName = "Handle - Deve extrair extensão corretamente com Path.GetExtension")]
        [InlineData("documento.pdf", ".pdf")]
        [InlineData("imagem.JPG", ".JPG")]
        [InlineData("arquivo.tar.gz", ".gz")]
        [InlineData("sem_extensao", "")]
        [InlineData("com.multiplos.pontos.docx", ".docx")]
        public async Task Handle_Deve_Extrair_Extensao_Corretamente(string nomeOriginal, string extensaoEsperada)
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock(nomeOriginal, "application/octet-stream");
            var arquivoDto = new ArquivoDTO(nomeOriginal, codigo, TipoArquivo.Temp, "application/octet-stream", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            string? nomeArquivoCapturado = null;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback<string, Stream, string>((nome, stream, contentType) => nomeArquivoCapturado = nome)
                .ReturnsAsync("https://storage.example.com/arquivo");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(nomeArquivoCapturado);
            Assert.EndsWith(extensaoEsperada, nomeArquivoCapturado);
            Assert.Equal($"{codigo}{extensaoEsperada}", nomeArquivoCapturado);
        }

        #endregion

        #region Testes de Assincronismo

        [Fact(DisplayName = "Handle - Deve ser assíncrono")]
        public async Task Handle_Deve_Ser_Assincrono()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            var tarefa = _handler.Handle(command, CancellationToken.None);

            // Assert
            await Assert.IsType<Task<string>>(tarefa);
            var resultado = await tarefa;
            Assert.NotEmpty(resultado);
        }

        [Fact(DisplayName = "Handle - Deve aguardar ArmazenarTemporaria")]
        public async Task Handle_Deve_Aguardar_ArmazenarTemporaria()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);
            var urlEsperada = "https://storage.example.com/arquivo.pdf";

            var foiChamado = false;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Returns(async () =>
                {
                    foiChamado = true;
                    await Task.Delay(10);
                    return urlEsperada;
                });

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(foiChamado);
            Assert.Equal(urlEsperada, resultado);
        }

        #endregion

        #region Testes de CancellationToken

        [Fact(DisplayName = "Handle - Deve respeitar CancellationToken")]
        public async Task Handle_Deve_Respeitar_CancellationToken()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);
            var cancellationToken = CancellationToken.None;

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            var resultado = await _handler.Handle(command, cancellationToken);

            // Assert
            Assert.NotEmpty(resultado);
        }

        #endregion

        #region Testes de Construtor

        [Fact(DisplayName = "Construtor - Deve validar ServicoArmazenamento nulo")]
        public void Construtor_Deve_Validar_ServicoArmazenamento_Nulo()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(null!));

            Assert.Equal("servicoArmazenamento", excecao.ParamName);
        }

        [Fact(DisplayName = "Construtor - Deve criar instância com parâmetro válido")]
        public void Construtor_Deve_Criar_Instancia_Com_Parametro_Valido()
        {
            // Act
            var handler = new ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(_servicoArmazenamentoMock.Object);

            // Assert
            Assert.NotNull(handler);
        }

        #endregion

        #region Testes de Implementação

        [Fact(DisplayName = "Implementação - Deve implementar IRequestHandler<ArmazenarArquivoTemporarioServicoArmazenamentoCommand, string>")]
        public void Deve_Implementar_IRequestHandler()
        {
            // Arrange & Act
            var interfaces = typeof(ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler).GetInterfaces();

            // Assert
            Assert.Contains(interfaces, i => 
                i.Name.Contains("IRequestHandler") && 
                i.GenericTypeArguments.Length == 2 &&
                i.GenericTypeArguments[0] == typeof(ArmazenarArquivoTemporarioServicoArmazenamentoCommand) &&
                i.GenericTypeArguments[1] == typeof(string));
        }

        [Fact(DisplayName = "Implementação - Deve usar primary constructor")]
        public void Deve_Usar_Primary_Constructor()
        {
            // Act
            var handler = new ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandler(_servicoArmazenamentoMock.Object);

            // Assert
            Assert.NotNull(handler);
        }

        #endregion

        #region Testes de Ordem de Operações

        [Fact(DisplayName = "Ordem - Deve extrair extensão do nome do arquivo antes de chamar OpenReadStream")]
        public async Task Ordem_Deve_Extrair_Extensao_Antes_De_OpenReadStream()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var ordem = new List<string>();

            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns("arquivo.pdf");
            formFileMock.Setup(f => f.OpenReadStream())
                .Callback(() => ordem.Add("OpenReadStream"))
                .Returns(new MemoryStream());

            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .Callback(() => ordem.Add("ArmazenarTemporaria"))
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(2, ordem.Count);
            Assert.Equal("OpenReadStream", ordem[0]);
            Assert.Equal("ArmazenarTemporaria", ordem[1]);
        }

        #endregion

        #region Testes de TipoArquivo

        [Fact(DisplayName = "Comportamento - Deve armazenar arquivo com TipoArquivo.Temp")]
        public async Task Comportamento_Deve_Armazenar_Com_TipoArquivo_Temp()
        {
            // Arrange
            var codigo = Guid.NewGuid();
            var formFileMock = CriarFormFileMock("arquivo.pdf", "application/pdf");
            var arquivoDto = new ArquivoDTO("arquivo.pdf", codigo, TipoArquivo.Temp, "application/pdf", formFileMock.Object);
            var command = new ArmazenarArquivoTemporarioServicoArmazenamentoCommand(arquivoDto);

            Assert.Equal(TipoArquivo.Temp, arquivoDto.Tipo);

            _servicoArmazenamentoMock
                .Setup(s => s.ArmazenarTemporaria(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("https://storage.example.com/arquivo.pdf");

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEmpty(resultado);
        }

        #endregion

        #region Métodos Auxiliares

        private static Mock<IFormFile> CriarFormFileMock(
            string nomeArquivo, 
            string contentType,
            byte[]? conteudo = null)
        {
            var formFileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(conteudo ?? "Conteúdo padrão"u8.ToArray());

            formFileMock.Setup(f => f.FileName).Returns(nomeArquivo);
            formFileMock.Setup(f => f.ContentType).Returns(contentType);
            formFileMock.Setup(f => f.Length).Returns(stream.Length);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);

            return formFileMock;
        }

        #endregion
    }
}
