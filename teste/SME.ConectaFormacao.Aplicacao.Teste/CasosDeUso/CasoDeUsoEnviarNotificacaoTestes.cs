using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Email;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarNotificacaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEnviarNotificacao _casoDeUso;

        public CasoDeUsoEnviarNotificacaoTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEnviarNotificacao(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve enviar notificação com sucesso quando dados são válidos")]
        public async Task Executar_Deve_Enviar_Notificacao_Com_Sucesso_Quando_Dados_Validos()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Título da Notificação",
                Usuarios = ["user1", "user2"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve passar dados corretos para EnviarNotificacaoCommand")]
        public async Task Executar_Deve_Passar_Dados_Corretos_Para_EnviarNotificacaoCommand()
        {
            // Arrange
            const long id = 42;
            const string titulo = "Notificação de Teste";
            var usuarios = new[] { "usuario1", "usuario2", "usuario3" };

            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = id,
                Titulo = titulo,
                Usuarios = usuarios
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.NotNull(commandCapturado.Notificacao);
            Assert.Equal(id, commandCapturado.Notificacao.Id);
            Assert.Equal(titulo, commandCapturado.Notificacao.Titulo);
            Assert.Equal(usuarios, commandCapturado.Notificacao.Usuarios);
        }

        [Fact(DisplayName = "Executar - Deve retornar true quando envio bem-sucedido")]
        public async Task Executar_Deve_Retornar_True_Quando_Envio_Bem_Sucedido()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Test Notification",
                Usuarios = ["test_user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            Assert.IsType<bool>(resultado);
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

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando NotificacaoSignalRDTO é null após desserialização")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Notificacao_Null_Apos_Desserializacao()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit("null");

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.DADOS_ENVIO_NOTIFICACAO_NAO_LOCALIZADO, excecao.Message);
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException com mensagem correta quando DTO é null")]
        public async Task Executar_Deve_Lancar_NegocioException_Com_Mensagem_Correta_Quando_Dto_Null()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit(string.Empty);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagemRabbit));

            Assert.Equal(MensagemNegocio.DADOS_ENVIO_NOTIFICACAO_NAO_LOCALIZADO, excecao.Message);
            Assert.Single(excecao.Mensagens);
            Assert.Equal(MensagemNegocio.DADOS_ENVIO_NOTIFICACAO_NAO_LOCALIZADO, excecao.Mensagens.First());
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

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Async Test",
                Usuarios = ["async_user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
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
                typeof(CasoDeUsoEnviarNotificacao)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEnviarNotificacao deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoEnviarNotificacao")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEnviarNotificacao)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEnviarNotificacao"),
                "CasoDeUsoEnviarNotificacao deve implementar ICasoDeUsoEnviarNotificacao");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Assert
            var casoDeUso = new CasoDeUsoEnviarNotificacao(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoEnviarNotificacao>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Title",
                Usuarios = ["user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Title",
                Usuarios = ["user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve extrair DTO da MensagemRabbit corretamente")]
        public async Task Executar_Deve_Extrair_Dto_Da_Mensagem_Corretamente()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 123,
                Titulo = "Extract Test",
                Usuarios = ["extract_user1", "extract_user2"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado?.Notificacao);
            Assert.NotNull(commandCapturado.Notificacao.Titulo);
            Assert.NotNull(commandCapturado.Notificacao.Usuarios);
        }

        [Fact(DisplayName = "Executar - Deve mapear Id da notificação corretamente")]
        public async Task Executar_Deve_Mapear_Id_Da_Notificacao()
        {
            // Arrange
            const long idEsperado = 999;
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = idEsperado,
                Titulo = "Notification Title",
                Usuarios = ["user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(idEsperado, commandCapturado.Notificacao.Id);
        }

        [Fact(DisplayName = "Executar - Deve mapear Titulo da notificação corretamente")]
        public async Task Executar_Deve_Mapear_Titulo_Da_Notificacao()
        {
            // Arrange
            const string tituloEsperado = "Título da Notificação Mapeado";
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = tituloEsperado,
                Usuarios = ["user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(tituloEsperado, commandCapturado.Notificacao.Titulo);
        }

        [Fact(DisplayName = "Executar - Deve mapear Usuarios da notificação corretamente")]
        public async Task Executar_Deve_Mapear_Usuarios_Da_Notificacao()
        {
            // Arrange
            var usuariosEsperados = new[] { "usuario1", "usuario2", "usuario3", "usuario4" };
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Title",
                Usuarios = usuariosEsperados
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(usuariosEsperados, commandCapturado.Notificacao.Usuarios);
        }

        [Fact(DisplayName = "Executar - Deve mapear DataHora da notificação corretamente")]
        public async Task Executar_Deve_Mapear_DataHora_Da_Notificacao()
        {
            // Arrange
            var dataEsperada = DateTime.UtcNow;
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Title",
                Usuarios = ["user"],
                DataHora = dataEsperada
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(dataEsperada, commandCapturado.Notificacao.DataHora);
        }

        [Fact(DisplayName = "Executar - Deve enviar comando com parâmetros nomeados corretos")]
        public async Task Executar_Deve_Enviar_Comando_Com_Parametros_Nomeados()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Test",
                Usuarios = ["user"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            bool commandoEnviado = false;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) => commandoEnviado = true)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(commandoEnviado);
        }

        [Fact(DisplayName = "Executar - Deve manter integridade dos dados através da serialização")]
        public async Task Executar_Deve_Manter_Integridade_Dos_Dados_Atraves_Da_Serializacao()
        {
            // Arrange
            var notificacaoDtoOriginal = new NotificacaoSignalRDTO
            {
                Id = 55,
                Titulo = "Notificação com Integridade",
                Usuarios = ["user_integridade_1", "user_integridade_2", "user_integridade_3"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDtoOriginal);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado?.Notificacao);
            var notificacaoRecuperada = commandCapturado.Notificacao;
            Assert.Equal(notificacaoDtoOriginal.Id, notificacaoRecuperada.Id);
            Assert.Equal(notificacaoDtoOriginal.Titulo, notificacaoRecuperada.Titulo);
            Assert.Equal(notificacaoDtoOriginal.Usuarios, notificacaoRecuperada.Usuarios);
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor corretamente")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Corretamente()
        {
            // Arrange & Act
            var casoDeUso = new CasoDeUsoEnviarNotificacao(_mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var baseType = typeof(CasoDeUsoEnviarNotificacao).BaseType;
            Assert.NotNull(baseType);
            Assert.Contains("CasoDeUsoAbstrato", baseType.Name);
        }

        [Fact(DisplayName = "Executar - Deve lidar com múltiplos usuários corretamente")]
        public async Task Executar_Deve_Lidar_Com_Multiplos_Usuarios()
        {
            // Arrange
            var usuariosMultiplos = new[] 
            { 
                "user1", "user2", "user3", "user4", "user5",
                "user6", "user7", "user8", "user9", "user10"
            };

            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Notificação com Múltiplos Usuários",
                Usuarios = usuariosMultiplos
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(usuariosMultiplos.Length, commandCapturado.Notificacao.Usuarios.Length);
            Assert.Equal(usuariosMultiplos, commandCapturado.Notificacao.Usuarios);
        }

        [Fact(DisplayName = "Executar - Deve lidar com usuario único corretamente")]
        public async Task Executar_Deve_Lidar_Com_Usuario_Unico()
        {
            // Arrange
            var notificacaoDto = new NotificacaoSignalRDTO
            {
                Id = 1,
                Titulo = "Notificação Usuário Único",
                Usuarios = ["usuario_unico"]
            };

            var mensagemRabbit = CriarMensagemRabbitComNotificacaoDto(notificacaoDto);

            EnviarNotificacaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarNotificacaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarNotificacaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Single(commandCapturado.Notificacao.Usuarios);
            Assert.Equal("usuario_unico", commandCapturado.Notificacao.Usuarios[0]);
        }

        private static MensagemRabbit CriarMensagemRabbitComNotificacaoDto(NotificacaoSignalRDTO notificacaoDto)
        {
            var json = notificacaoDto.ObjetoParaJson();
            return new MensagemRabbit(json);
        }
    }
}
