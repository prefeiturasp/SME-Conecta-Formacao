using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarEmailTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEnviarEmail _casoDeUso;

        public CasoDeUsoEnviarEmailTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEnviarEmail(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve enviar e-mail com sucesso quando dados são válidos")]
        public async Task Executar_Deve_Enviar_Email_Com_Sucesso_Quando_Dados_Validos()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "João da Silva",
                EmailDestinatario = "joao@example.com",
                Titulo = "Assunto do Email",
                Texto = "<h1>Conteúdo HTML</h1>"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve passar dados corretos para EnviarEmailCommand")]
        public async Task Executar_Deve_Passar_Dados_Corretos_Para_EnviarEmailCommand()
        {
            // Arrange
            const string nomeDestinatario = "Maria Silva";
            const string emailDestinatario = "maria@example.com";
            const string titulo = "Título do Email";
            const string texto = "<p>Corpo do email</p>";

            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = nomeDestinatario,
                EmailDestinatario = emailDestinatario,
                Titulo = titulo,
                Texto = texto
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(nomeDestinatario, commandCapturado.NomeDestinatario);
            Assert.Equal(emailDestinatario, commandCapturado.EmailDestinatario);
            Assert.Equal(titulo, commandCapturado.Assunto);
            Assert.Equal(texto, commandCapturado.MensagemHtml);
        }

        [Fact(DisplayName = "Executar - Deve retornar true quando envio bem-sucedido")]
        public async Task Executar_Deve_Retornar_True_Quando_Envio_Bem_Sucedido()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Test User",
                EmailDestinatario = "test@example.com",
                Titulo = "Test Title",
                Texto = "Test Content"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            Assert.IsType<bool>(resultado);
        }
      
        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando Mensagem é null")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Mensagem_Null()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _casoDeUso.Executar(mensagemRabbit));
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando desserialização falha")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Desserializacao_Falha()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit("json inválido");

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando EnviarEmailDto é null após desserialização")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Dto_Null_Apos_Desserializacao()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit(string.Empty);

            // Act & Assert - JsonParaObjeto retorna null para string vazia
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO, excecao.Message);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Async Test",
                EmailDestinatario = "async@example.com",
                Titulo = "Async Email",
                Texto = "Async Content"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var tarefa = _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<bool>>(tarefa);

            var resultado = await tarefa;
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEnviarEmail)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEnviarEmail deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoEnviarEmail")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEnviarEmail)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEnviarEmail"),
                "CasoDeUsoEnviarEmail deve implementar ICasoDeUsoEnviarEmail");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Assert
            var casoDeUso = new CasoDeUsoEnviarEmail(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoEnviarEmail>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve enviar comando com parâmetros nomeados corretos")]
        public async Task Executar_Deve_Enviar_Comando_Com_Parametros_Nomeados()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Test",
                EmailDestinatario = "test@test.com",
                Titulo = "Titulo",
                Texto = "Texto"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            bool commandoEnviado = false;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) => commandoEnviado = true)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(commandoEnviado);
        }

        [Fact(DisplayName = "Executar - Deve mapear NomeDestinatario do DTO para NomeDestinatario do Command")]
        public async Task Executar_Deve_Mapear_NomeDestinatario()
        {
            // Arrange
            const string nomeEsperado = "João Silva Oliveira";
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = nomeEsperado,
                EmailDestinatario = "joao@test.com",
                Titulo = "Title",
                Texto = "Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(nomeEsperado, commandCapturado.NomeDestinatario);
        }

        [Fact(DisplayName = "Executar - Deve mapear EmailDestinatario do DTO para EmailDestinatario do Command")]
        public async Task Executar_Deve_Mapear_EmailDestinatario()
        {
            // Arrange
            const string emailEsperado = "destinatario@empresa.com.br";
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Name",
                EmailDestinatario = emailEsperado,
                Titulo = "Title",
                Texto = "Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(emailEsperado, commandCapturado.EmailDestinatario);
        }

        [Fact(DisplayName = "Executar - Deve mapear Titulo do DTO para Assunto do Command")]
        public async Task Executar_Deve_Mapear_Titulo_Para_Assunto()
        {
            // Arrange
            const string tituloEsperado = "Assunto Importante do Email";
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Name",
                EmailDestinatario = "test@test.com",
                Titulo = tituloEsperado,
                Texto = "Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(tituloEsperado, commandCapturado.Assunto);
        }

        [Fact(DisplayName = "Executar - Deve mapear Texto do DTO para MensagemHtml do Command")]
        public async Task Executar_Deve_Mapear_Texto_Para_MensagemHtml()
        {
            // Arrange
            const string textoEsperado = "<html><body><h1>Conteúdo</h1></body></html>";
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Name",
                EmailDestinatario = "test@test.com",
                Titulo = "Title",
                Texto = textoEsperado
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(textoEsperado, commandCapturado.MensagemHtml);
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Name",
                EmailDestinatario = "test@test.com",
                Titulo = "Title",
                Texto = "Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Name",
                EmailDestinatario = "test@test.com",
                Titulo = "Title",
                Texto = "Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException com mensagem quando DTO é null")]
        public async Task Executar_Deve_Lancar_NegocioException_Com_Mensagem_Quando_Dto_Null()
        {
            // Arrange
            // Simula um JSON que desserializa para null
            var mensagemRabbit = new MensagemRabbit("null");

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO, excecao.Message);
            Assert.Single(excecao.Mensagens);
            Assert.Equal(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO, excecao.Mensagens.First());
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException com status code BadRequest")]
        public async Task Executar_Deve_Lancar_NegocioException_Com_Status_BadRequest()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit("null");

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(400, excecao.StatusCode);
        }

        [Fact(DisplayName = "Executar - Deve extrair DTO da MensagemRabbit corretamente")]
        public async Task Executar_Deve_Extrair_Dto_Da_Mensagem_Corretamente()
        {
            // Arrange
            var enviarEmailDto = new EnviarEmailDto
            {
                NomeDestinatario = "Extract Test",
                EmailDestinatario = "extract@test.com",
                Titulo = "Extract Title",
                Texto = "Extract Text"
            };

            var mensagemRabbit = CriarMensagemRabbitComEnviarEmailDto(enviarEmailDto);

            EnviarEmailCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.All([commandCapturado.NomeDestinatario, commandCapturado.EmailDestinatario, commandCapturado.Assunto, commandCapturado.MensagemHtml],
                item => Assert.NotNull(item));
        }

        private static MensagemRabbit CriarMensagemRabbitComEnviarEmailDto(EnviarEmailDto enviarEmailDto)
        {
            var json = enviarEmailDto.ObjetoParaJson();
            return new MensagemRabbit(json);
        }
    }
}
