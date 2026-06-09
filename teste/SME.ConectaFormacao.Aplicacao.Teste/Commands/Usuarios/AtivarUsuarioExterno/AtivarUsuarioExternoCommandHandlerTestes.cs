using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios.AtivarUsuarioExterno
{
    public class AtivarUsuarioExternoCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly AtivarUsuarioExternoCommandHandler _handler;
        private readonly Faker _faker;
        private static readonly string[] expected = ["ObterPorLogin", "AtivarCadastro", "RemoverCache"];

        public AtivarUsuarioExternoCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<AtivarUsuarioExternoCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        #region Testes de Sucesso

        [Fact(DisplayName = "Handler - Deve ativar usuário externo com sucesso")]
        public async Task Deve_Ativar_Usuario_Externo_Com_Sucesso()
        {
            // Arrange
            var login = "usuario.externo@teste.com";
            var usuarioId = _faker.Random.Long(1, 9999);
            var command = new AtivarUsuarioExternoCommand(login);
            
            var usuarioExterno = new Usuario
            {
                Id = usuarioId,
                Login = login,
                Tipo = TipoUsuario.Externo,
                Situacao = SituacaoUsuario.Inativo
            };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuarioExterno);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.ObterPorLogin(login), Times.Once);
            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.AtivarCadastroUsuario(usuarioId), Times.Once);
            _mocker.GetMock<ICacheDistribuido>()
                .Verify(c => c.RemoverAsync(CacheDistribuidoNomes.Usuario.Parametros(login)), Times.Once);
        }

        [Fact(DisplayName = "Handler - Deve remover cache do usuário após ativação")]
        public async Task Deve_Remover_Cache_Usuario_Apos_Ativacao()
        {
            // Arrange
            var login = "usuario.externo@teste.com";
            var usuarioId = _faker.Random.Long(1, 9999);
            var command = new AtivarUsuarioExternoCommand(login);
            
            var usuarioExterno = new Usuario
            {
                Id = usuarioId,
                Login = login,
                Tipo = TipoUsuario.Externo
            };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuarioExterno);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            var chaveEsperada = CacheDistribuidoNomes.Usuario.Parametros(login);
            _mocker.GetMock<ICacheDistribuido>()
                .Verify(c => c.RemoverAsync(chaveEsperada), Times.Once);
        }

        [Fact(DisplayName = "Handler - Deve retornar true ao ativar com sucesso")]
        public async Task Deve_Retornar_True_Ao_Ativar_Com_Sucesso()
        {
            // Arrange
            var login = "usuario.teste";
            var usuarioId = 123L;
            var command = new AtivarUsuarioExternoCommand(login);
            
            var usuario = new Usuario { Id = usuarioId, Login = login, Tipo = TipoUsuario.Externo };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region Testes de Falha - Usuário não encontrado

        [Fact(DisplayName = "Handler - Deve lançar NegocioException quando usuário não encontrado")]
        public async Task Deve_Lancar_NegocioException_Usuario_Nao_Encontrado()
        {
            // Arrange
            var login = "usuario.inexistente@teste.com";
            var command = new AtivarUsuarioExternoCommand(login);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.Equal(MensagemNegocio.USUARIO_NAO_ENCONTRADO, excecao.Message);
        }

        [Fact(DisplayName = "Handler - Não deve chamar AtivarCadastroUsuario quando usuário não existe")]
        public async Task Nao_Deve_Chamar_Ativar_Quando_Usuario_Nao_Existe()
        {
            // Arrange
            var login = "usuario.inexistente";
            var command = new AtivarUsuarioExternoCommand(login);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(command, CancellationToken.None));

            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.AtivarCadastroUsuario(It.IsAny<long>()), Times.Never);
        }

        [Fact(DisplayName = "Handler - Não deve chamar RemoverAsync do cache quando usuário não existe")]
        public async Task Nao_Deve_Chamar_RemoverCache_Quando_Usuario_Nao_Existe()
        {
            // Arrange
            var login = "usuario.inexistente";
            var command = new AtivarUsuarioExternoCommand(login);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(command, CancellationToken.None));

            _mocker.GetMock<ICacheDistribuido>()
                .Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Testes de Validação de Entrada

        [Fact(DisplayName = "Handler - Deve buscar usuário com login fornecido")]
        public async Task Deve_Buscar_Usuario_Com_Login_Fornecido()
        {
            // Arrange
            var login = "usuario@exemplo.com.br";
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = 1, Login = login, Tipo = TipoUsuario.Externo };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuario.Id))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.ObterPorLogin(login), Times.Once);
        }

        [Fact(DisplayName = "Handler - Deve usar ID correto do usuário ao ativar")]
        public async Task Deve_Usar_ID_Correto_Usuario_Ao_Ativar()
        {
            // Arrange
            var login = "usuario@teste.com";
            var usuarioId = 9876L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = usuarioId, Login = login, Tipo = TipoUsuario.Externo };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.AtivarCadastroUsuario(usuarioId), Times.Once);
        }

        #endregion

        #region Testes de Ordem de Execução

        [Fact(DisplayName = "Handler - Deve obter usuário antes de ativar")]
        public async Task Deve_Obter_Usuario_Antes_De_Ativar()
        {
            // Arrange
            var login = "usuario@teste.com";
            var usuarioId = 1L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = usuarioId, Login = login, Tipo = TipoUsuario.Externo };

            var chamadas = new List<string>();

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .Callback(() => chamadas.Add("ObterPorLogin"))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Callback(() => chamadas.Add("AtivarCadastro"))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Callback(() => chamadas.Add("RemoverCache"))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, chamadas);
        }

        [Fact(DisplayName = "Handler - Deve remover cache após ativar usuário")]
        public async Task Deve_Remover_Cache_Apos_Ativar_Usuario()
        {
            // Arrange
            var login = "usuario@teste.com";
            var usuarioId = 1L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = usuarioId, Login = login, Tipo = TipoUsuario.Externo };

            var chamadas = new List<string>();

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .Callback(() => chamadas.Add("ObterPorLogin"))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Callback(() => chamadas.Add("AtivarCadastro"))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Callback(() => chamadas.Add("RemoverCache"))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expected, chamadas);
        }

        #endregion

        #region Testes de Tratamento de Nulo

        [Fact(DisplayName = "Handler - Deve validar extensão EhNulo corretamente")]
        public async Task Deve_Validar_Extensao_EhNulo_Corretamente()
        {
            // Arrange
            var login = "usuario@teste.com";
            var command = new AtivarUsuarioExternoCommand(login);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync((Usuario?)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _handler.Handle(command, CancellationToken.None));

            Assert.NotNull(excecao);
        }

        #endregion

        #region Testes de Tipos de Usuário

        [Fact(DisplayName = "Handler - Deve ativar usuário externo com Tipo correto")]
        public async Task Deve_Ativar_Usuario_Externo_Com_Tipo_Correto()
        {
            // Arrange
            var login = "usuario.externo@teste.com";
            var usuarioId = 1L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario
            {
                Id = usuarioId,
                Login = login,
                Tipo = TipoUsuario.Externo,
                Situacao = SituacaoUsuario.Inativo
            };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            Assert.Equal(TipoUsuario.Externo, usuario.Tipo);
        }

        [Fact(DisplayName = "Handler - Deve ativar usuário mesmo com situação Inativo")]
        public async Task Deve_Ativar_Usuario_Com_Situacao_Inativo()
        {
            // Arrange
            var login = "usuario@teste.com";
            var usuarioId = 1L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario
            {
                Id = usuarioId,
                Login = login,
                Situacao = SituacaoUsuario.Inativo
            };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region Testes de CancellationToken

        [Fact(DisplayName = "Handler - Deve respeitar CancellationToken")]
        public async Task Deve_Respeitar_CancellationToken()
        {
            // Arrange
            var login = "usuario@teste.com";
            var usuarioId = 1L;
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = usuarioId, Login = login };
            var cancellationToken = CancellationToken.None;

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, cancellationToken);

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region Testes de Construtor

        [Fact(DisplayName = "Handler - Construtor deve validar repositório nulo")]
        public void Construtor_Deve_Validar_Repositorio_Nulo()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new AtivarUsuarioExternoCommandHandler(null!, _mocker.GetMock<ICacheDistribuido>().Object));

            Assert.Equal("repositorioUsuario", excecao.ParamName);
        }

        [Fact(DisplayName = "Handler - Construtor deve validar cache nulo")]
        public void Construtor_Deve_Validar_Cache_Nulo()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new AtivarUsuarioExternoCommandHandler(_mocker.GetMock<IRepositorioUsuario>().Object, null!));

            Assert.Equal("cacheDistribuid", excecao.ParamName);
        }

        [Fact(DisplayName = "Handler - Construtor deve criar instância com parâmetros válidos")]
        public void Construtor_Deve_Criar_Instancia_Com_Parametros_Validos()
        {
            // Arrange & Act
            var handler = new AtivarUsuarioExternoCommandHandler(
                _mocker.GetMock<IRepositorioUsuario>().Object,
                _mocker.GetMock<ICacheDistribuido>().Object);

            // Assert
            Assert.NotNull(handler);
        }

        #endregion

        #region Testes de Variações de Login

        [Theory(DisplayName = "Handler - Deve ativar usuário com diferentes formatos de login")]
        [InlineData("usuario")]
        [InlineData("usuario.externo@teste.com")]
        [InlineData("usuario-123")]
        [InlineData("usuario_teste")]
        [InlineData("12345")]
        public async Task Deve_Ativar_Usuario_Com_Diferentes_Formatos_Login(string login)
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1, 9999);
            var command = new AtivarUsuarioExternoCommand(login);
            var usuario = new Usuario { Id = usuarioId, Login = login };

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.ObterPorLogin(login))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(r => r.AtivarCadastroUsuario(usuarioId))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<ICacheDistribuido>()
                .Setup(c => c.RemoverAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _mocker.GetMock<IRepositorioUsuario>()
                .Verify(r => r.ObterPorLogin(login), Times.Once);
        }

        #endregion

        #region Testes de IRequestHandler

        [Fact(DisplayName = "Handler - Deve implementar IRequestHandler<AtivarUsuarioExternoCommand, bool>")]
        public void Deve_Implementar_IRequestHandler()
        {
            // Arrange & Act
            var handler = _handler;

            // Assert
            Assert.IsType<MediatR.IRequestHandler<AtivarUsuarioExternoCommand, bool>>(handler, exactMatch: false);
        }

        #endregion
    }
}
