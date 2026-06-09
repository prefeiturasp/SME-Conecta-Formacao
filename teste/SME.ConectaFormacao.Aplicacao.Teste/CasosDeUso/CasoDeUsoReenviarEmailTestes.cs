using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoReenviarEmailTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoReenviarEmail _casoDeUso;

        public CasoDeUsoReenviarEmailTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoReenviarEmail(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar true quando envio de email bem-sucedido")]
        public async Task Executar_Deve_Retornar_True_Quando_Envio_Bem_Sucedido()
        {
            // Arrange
            const string loginUsuario = "usuario.teste@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar false quando envio de email falhar")]
        public async Task Executar_Deve_Retornar_False_Quando_Envio_Falhar()
        {
            // Arrange
            const string loginUsuario = "usuario.teste@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Executar - Deve enviar comando com login correto")]
        public async Task Executar_Deve_Enviar_Comando_Com_Login_Correto()
        {
            // Arrange
            const string loginUsuario = "joao.silva@email.com";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(loginUsuario, commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var tarefa = _casoDeUso.Executar(loginUsuario);

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
                typeof(CasoDeUsoReenviarEmail)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoReenviarEmail deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoReenviarEmail")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoReenviarEmail)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoReenviarEmail"),
                "CasoDeUsoReenviarEmail deve implementar ICasoDeUsoReenviarEmail");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Assert
            var casoDeUso = new CasoDeUsoReenviarEmail(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoReenviarEmail>(casoDeUso);
            Assert.IsType<ICasoDeUsoReenviarEmail>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve preservar tipo de retorno bool")]
        public async Task Executar_Deve_Preservar_Tipo_Retorno_Bool()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.IsType<bool>(resultado);
        }

        [Fact(DisplayName = "Executar - Deve mapear login corretamente para command")]
        public async Task Executar_Deve_Mapear_Login_Corretamente()
        {
            // Arrange
            const string loginEsperado = "maria.santos@dominio.com.br";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginEsperado);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(loginEsperado, commandCapturado.Login);
            Assert.NotEmpty(commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve criar instância correta do Command")]
        public async Task Executar_Deve_Criar_Instancia_Correta_Command()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.IsType<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(commandCapturado);
        }

        [Fact(DisplayName = "Executar - Deve comportar-se corretamente com login vazio")]
        public async Task Executar_Deve_Comportar_Corretamente_Com_Login_Vazio()
        {
            // Arrange
            const string loginUsuario = "";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.True(resultado);
            Assert.NotNull(commandCapturado);
            Assert.Equal(loginUsuario, commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve comportar-se corretamente com login contendo espaços")]
        public async Task Executar_Deve_Comportar_Corretamente_Com_Login_Espacos()
        {
            // Arrange
            const string loginUsuario = "usuario com espacos@email.com";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(loginUsuario, commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve comportar-se corretamente com login muito longo")]
        public async Task Executar_Deve_Comportar_Corretamente_Com_Login_Muito_Longo()
        {
            // Arrange
            const string loginUsuario = "usuario.com.nome.muito.longo.e.complexo.12345@empresa.com.br";
            EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(loginUsuario, commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve executar com sucesso múltiplas vezes")]
        public async Task Executar_Deve_Executar_Multiplas_Vezes()
        {
            // Arrange
            var logins = new[] { "usuario1@email.com", "usuario2@email.com", "usuario3@email.com" };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultados = new List<bool>();
            foreach (var login in logins)
            {
                resultados.Add(await _casoDeUso.Executar(login));
            }

            // Assert
            Assert.All(resultados, Assert.True);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(logins.Length),
                "Mediator.Send deve ser chamado para cada execução");
        }

        [Fact(DisplayName = "Executar - Deve retornar valor enviado por Send diretamente")]
        public async Task Executar_Deve_Retornar_Valor_Enviado_Por_Send_Diretamente()
        {
            // Arrange
            const string loginUsuario = "usuario@email.com";
            const bool valorEsperado = true;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(valorEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(loginUsuario);

            // Assert
            Assert.Equal(valorEsperado, resultado);
        }
    }
}
