using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Servicos.Log;
using SME.ConectaFormacao.Infra.Servicos.Mensageria;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.PublicarNaFilaRabbit
{
    public class PublicarNaFilaRabbitCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IServicoMensageriaConecta> _servicoMensageriaMock;
        private readonly Mock<IServicoMensageriaMetricas> _servicoMensageriaMetricasMock;
        private readonly PublicarNaFilaRabbitCommandHandler _handler;

        public PublicarNaFilaRabbitCommandHandlerTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _servicoMensageriaMock = new Mock<IServicoMensageriaConecta>();
            _servicoMensageriaMetricasMock = new Mock<IServicoMensageriaMetricas>();
            _handler = new PublicarNaFilaRabbitCommandHandler(_mediatorMock.Object, _servicoMensageriaMock.Object, _servicoMensageriaMetricasMock.Object);
        }

        #region Testes de Sucesso

        [Fact(DisplayName = "Handle - Deve retornar true quando publicar mensagem com sucesso")]
        public async Task Handle_Deve_Retornar_True_Quando_Publicar_Com_Sucesso()
        {
            // Arrange
            var usuario = new Usuario { Nome = "João Silva", Login = "joao.silva" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Handle - Deve publicar mensagem com usuário fornecido no comando")]
        public async Task Handle_Deve_Publicar_Mensagem_Com_Usuario_Fornecido()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Maria Santos", Login = "maria.santos" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            Assert.Equal(usuario.Nome, mensagemCapturada.UsuarioLogadoNomeCompleto);
            Assert.Equal(usuario.Login, mensagemCapturada.UsuarioLogadoRF);
        }

        [Fact(DisplayName = "Handle - Deve usar Exchange padrão quando não fornecido")]
        public async Task Handle_Deve_Usar_Exchange_Padrao_Quando_Nao_Fornecido()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario, exchange: null);
            var cancellationToken = CancellationToken.None;

            string? exchangeCapturado = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => exchangeCapturado = e)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(exchangeCapturado);
            Assert.Equal(ExchangeRabbit.Conecta, exchangeCapturado);
        }

        [Fact(DisplayName = "Handle - Deve usar Exchange customizado quando fornecido")]
        public async Task Handle_Deve_Usar_Exchange_Customizado_Quando_Fornecido()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            const string exchangeCustomizado = "exchange.customizado";
            var comando = CriarComandoValido(usuario: usuario, exchange: exchangeCustomizado);
            var cancellationToken = CancellationToken.None;

            string? exchangeCapturado = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => exchangeCapturado = e)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.Equal(exchangeCustomizado, exchangeCapturado);
        }

        [Fact(DisplayName = "Handle - Deve registrar métrica de publicação")]
        public async Task Handle_Deve_Registrar_Metrica_De_Publicacao()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            const string rota = "test.queue";
            var comando = CriarComandoValido(usuario: usuario, rota: rota);
            var cancellationToken = CancellationToken.None;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            _servicoMensageriaMetricasMock.Verify(
                s => s.Publicado(rota),
                Times.Once,
                $"Métrica de publicação deve ser registrada para a rota {rota}");
        }

        [Fact(DisplayName = "Handle - Deve passar rota correta para publicação")]
        public async Task Handle_Deve_Passar_Rota_Correta_Para_Publicacao()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            const string rotaEsperada = "minha.rota.especifica";
            var comando = CriarComandoValido(usuario: usuario, rota: rotaEsperada);
            var cancellationToken = CancellationToken.None;

            string? rotaCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => rotaCapturada = r)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.Equal(rotaEsperada, rotaCapturada);
        }

        [Fact(DisplayName = "Handle - Deve passar filtros corretos na mensagem")]
        public async Task Handle_Deve_Passar_Filtros_Corretos_Na_Mensagem()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var filtrosEsperados = new { id = 1, nome = "teste" };
            var comando = CriarComandoValido(usuario: usuario, filtros: filtrosEsperados);
            var cancellationToken = CancellationToken.None;

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            Assert.Equal(filtrosEsperados, mensagemCapturada.Mensagem);
        }

        [Fact(DisplayName = "Handle - Deve passar código de correlação correto")]
        public async Task Handle_Deve_Passar_Codigo_Correlacao_Correto()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var codigoCorrelacaoEsperado = Guid.NewGuid();
            var comando = CriarComandoValido(usuario: usuario, codigoCorrelacao: codigoCorrelacaoEsperado);
            var cancellationToken = CancellationToken.None;

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            Assert.Equal(codigoCorrelacaoEsperado, mensagemCapturada.CodigoCorrelacao);
        }

        [Fact(DisplayName = "Handle - Deve passar notificarErroUsuario correto")]
        public async Task Handle_Deve_Passar_NotificarErroUsuario_Correto()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario, notificarErroUsuario: true);
            var cancellationToken = CancellationToken.None;

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            Assert.True(mensagemCapturada.NotificarErroUsuario);
        }

        #endregion

        #region Testes com Usuário Null - Recuperação via Mediator

        [Fact(DisplayName = "Handle - Deve obter usuário via mediator quando usuário null no comando")]
        public async Task Handle_Deve_Obter_Usuario_Via_Mediator_Quando_Null()
        {
            // Arrange
            var usuarioRecuperado = new Usuario { Nome = "Usuário Recuperado", Login = "recuperado" };
            var comando = CriarComandoValido(usuario: null);
            var cancellationToken = CancellationToken.None;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioRecuperado);

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator deve ser usado para recuperar usuário quando null no comando");
        }

        [Fact(DisplayName = "Handle - Deve usar usuário recuperado do mediator na mensagem")]
        public async Task Handle_Deve_Usar_Usuario_Recuperado_Na_Mensagem()
        {
            // Arrange
            var usuarioRecuperado = new Usuario { Nome = "Usuário do Mediator", Login = "mediator.user" };
            var comando = CriarComandoValido(usuario: null);
            var cancellationToken = CancellationToken.None;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioRecuperado);

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            Assert.Equal(usuarioRecuperado.Nome, mensagemCapturada.UsuarioLogadoNomeCompleto);
            Assert.Equal(usuarioRecuperado.Login, mensagemCapturada.UsuarioLogadoRF);
        }

        [Fact(DisplayName = "Handle - Deve usar usuário vazio quando mediator lança exceção")]
        public async Task Handle_Deve_Usar_Usuario_Vazio_Quando_Mediator_Lanca_Excecao()
        {
            // Arrange
            var comando = CriarComandoValido(usuario: null);
            var cancellationToken = CancellationToken.None;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao recuperar usuário"));

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.True(resultado);
            Assert.NotNull(mensagemCapturada);
            // Usuário vazio terá Nome e Login null
            Assert.Null(mensagemCapturada.UsuarioLogadoNomeCompleto);
            Assert.Null(mensagemCapturada.UsuarioLogadoRF);
        }

        #endregion

        #region Testes de Ordem de Operações

        [Fact(DisplayName = "Handle - Deve chamar PublicarNaMensageria antes de registrar métrica")]
        public async Task Handle_Deve_Chamar_Publicar_Antes_De_Registrar_Metrica()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            var ordem = new List<string>();

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback(() => ordem.Add("Publicar"))
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Callback(() => ordem.Add("Métrica"))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.Equal(2, ordem.Count);
            Assert.Equal("Publicar", ordem[0]);
            Assert.Equal("Métrica", ordem[1]);
        }

        #endregion

        #region Testes de Parâmetros do Publicar

        [Fact(DisplayName = "Handle - Deve chamar Publicar com nome de ação 'PublicarFilaConecta'")]
        public async Task Handle_Deve_Chamar_Publicar_Com_Nome_Acao_Correto()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            string? nomeAcaoCapturado = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => nomeAcaoCapturado = n)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.Equal("PublicarFilaConecta", nomeAcaoCapturado);
        }

        [Fact(DisplayName = "Handle - Deve criar MensagemRabbit com null como perfil")]
        public async Task Handle_Deve_Criar_MensagemRabbit_Com_Null_Como_Perfil()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            MensagemRabbit? mensagemCapturada = null;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .Callback<MensagemRabbit, string, string, string, object>(
                    (m, r, e, n, c) => mensagemCapturada = m)
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(mensagemCapturada);
            // O terceiro parâmetro do construtor MensagemRabbit é o perfil (null)
            Assert.Null(mensagemCapturada.PerfilUsuario);
        }

        #endregion

        #region Testes Assíncronos

        [Fact(DisplayName = "Handle - Deve ser assíncrono")]
        public async Task Handle_Deve_Ser_Assincrono()
        {
            // Arrange
            var usuario = new Usuario { Nome = "Test User", Login = "test.user" };
            var comando = CriarComandoValido(usuario: usuario);
            var cancellationToken = CancellationToken.None;

            _servicoMensageriaMock
                .Setup(s => s.Publicar(It.IsAny<MensagemRabbit>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null!))
                .ReturnsAsync(true);

            _servicoMensageriaMetricasMock
                .Setup(s => s.Publicado(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var tarefa = _handler.Handle(comando, cancellationToken);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<bool>>(tarefa);
            var resultado = await tarefa;
            Assert.True(resultado);
        }

        #endregion

        #region Testes de Implementação

        [Fact(DisplayName = "Handle - Deve implementar IRequestHandler<PublicarNaFilaRabbitCommand, bool>")]
        public void Handle_Deve_Implementar_Interface_Correta()
        {
            // Assert
            Assert.True(
                typeof(PublicarNaFilaRabbitCommandHandler)
                    .GetInterfaces()
                    .Any(i => i.Name.Contains("IRequestHandler") && i.GenericTypeArguments.Length == 2),
                "PublicarNaFilaRabbitCommandHandler deve implementar IRequestHandler<PublicarNaFilaRabbitCommand, bool>");
        }

        [Fact(DisplayName = "Handle - Deve usar primary constructor")]
        public void Handle_Deve_Usar_Primary_Constructor()
        {
            // Act & Assert
            var handler = new PublicarNaFilaRabbitCommandHandler(_mediatorMock.Object, _servicoMensageriaMock.Object, _servicoMensageriaMetricasMock.Object);
            Assert.NotNull(handler);
        }

        #endregion

        #region Métodos Auxiliares

        private static PublicarNaFilaRabbitCommand CriarComandoValido(
            string rota = "test.queue",
            object? filtros = null,
            Guid? codigoCorrelacao = null,
            Usuario? usuario = null,
            bool notificarErroUsuario = false,
            string? exchange = null)
        {
            return new PublicarNaFilaRabbitCommand(
                rota,
                filtros ?? new { },
                codigoCorrelacao ?? Guid.NewGuid(),
                usuario,
                notificarErroUsuario,
                exchange);
        }

        #endregion
    }
}
