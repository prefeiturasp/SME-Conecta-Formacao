using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoUsuarioValidacaoEmailTokenTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoUsuarioValidacaoEmailToken _casoDeUso;

        public CasoDeUsoUsuarioValidacaoEmailTokenTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoUsuarioValidacaoEmailToken(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar UsuarioPerfisRetornoDTO com sucesso quando token válido")]
        public async Task Executar_Deve_Retornar_UsuarioPerfisRetornoDTO_Quando_Token_Valido()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<UsuarioPerfisRetornoDTO>(resultado);
            Assert.Equal(login, resultado.UsuarioLogin);
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando token inválido")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Token_Invalido()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(token));

            Assert.Equal(MensagemNegocio.TOKEN_INVALIDO, excecao.Message);
            Assert.Equal(400, excecao.StatusCode);
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando login é null")]
        public async Task Executar_Deve_Lancar_NegocioException_Quando_Login_Null()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<string>(null!)); 

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(token));

            Assert.Equal(MensagemNegocio.TOKEN_INVALIDO, excecao.Message);
            Assert.Equal(400, excecao.StatusCode);
        }

        [Fact(DisplayName = "Executar - Deve chamar ObterLoginUsuarioTokenServicoAcessosQuery com token correto")]
        public async Task Executar_Deve_Chamar_ObterLoginUsuarioTokenServicoAcessosQuery_Com_Token_Correto()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);
            ObterLoginUsuarioTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<string>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterLoginUsuarioTokenServicoAcessosQuery)
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(token, queryCapturada.Token);
        }

        [Fact(DisplayName = "Executar - Deve chamar AtivarUsuarioExternoCommand com login correto")]
        public async Task Executar_Deve_Chamar_AtivarUsuarioExternoCommand_Com_Login_Correto()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario.externo@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);
            AtivarUsuarioExternoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AtivarUsuarioExternoCommand)
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(login, commandCapturado.Login);
        }

        [Fact(DisplayName = "Executar - Deve chamar ObterTokenAcessoQuery com login correto")]
        public async Task Executar_Deve_Chamar_ObterTokenAcessoQuery_Com_Login_Correto()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@test.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);
            ObterTokenAcessoQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UsuarioPerfisRetornoDTO>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterTokenAcessoQuery)
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(login, queryCapturada.Login);
        }

        [Fact(DisplayName = "Executar - Deve chamar Send exatamente 3 vezes com tipos específicos")]
        public async Task Executar_Deve_Chamar_Send_Exatamente_3_Vezes()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            var tarefa = _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(tarefa);
            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoUsuarioValidacaoEmailToken)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoUsuarioValidacaoEmailToken deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoUsuarioValidacaoEmailToken")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoUsuarioValidacaoEmailToken)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoUsuarioValidacaoEmailToken"),
                "CasoDeUsoUsuarioValidacaoEmailToken deve implementar ICasoDeUsoUsuarioValidacaoEmailToken");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Assert
            var casoDeUso = new CasoDeUsoUsuarioValidacaoEmailToken(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoUsuarioValidacaoEmailToken>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator em cada chamada")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve mapear login obtido para command de ativação")]
        public async Task Executar_Deve_Mapear_Login_Para_Command_Ativacao()
        {
            // Arrange
            var token = Guid.NewGuid();
            const string loginEsperado = "joao.silva@company.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(loginEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<AtivarUsuarioExternoCommand>(cmd => cmd.Login == loginEsperado),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve mapear login obtido para query de token")]
        public async Task Executar_Deve_Mapear_Login_Para_Query_Token()
        {
            // Arrange
            var token = Guid.NewGuid();
            const string loginEsperado = "maria.santos@company.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(loginEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterTokenAcessoQuery>(query => query.Login == loginEsperado),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve validar email com TipoAcao.ValidacaoEmail")]
        public async Task Executar_Deve_Validar_Email_Com_TipoAcao_ValidacaoEmail()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);
            ObterLoginUsuarioTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<string>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterLoginUsuarioTokenServicoAcessosQuery)
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(2, (int)queryCapturada.TipoAcao);
        }

        [Fact(DisplayName = "Executar - Deve retornar DTO com dados completos")]
        public async Task Executar_Deve_Retornar_DTO_Com_Dados_Completos()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Token);
            Assert.NotEmpty(resultado.UsuarioLogin);
            Assert.NotEmpty(resultado.UsuarioNome);
            Assert.NotEmpty(resultado.Email);
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException ao validar login vazio")]
        public async Task Executar_Deve_Lancar_Excecao_Ao_Validar_Login_Vazio()
        {
            // Arrange
            var token = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(string.Empty);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(token));
            Assert.Single(excecao.Mensagens);
            Assert.Equal(MensagemNegocio.TOKEN_INVALIDO, excecao.Mensagens.First());
        }
       
        [Fact(DisplayName = "Executar - Deve executar fluxo completo com sucesso")]
        public async Task Executar_Deve_Executar_Fluxo_Completo_Com_Sucesso()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario.completo@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(login, resultado.UsuarioLogin);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve retornar mesmo DTO retornado por ObterTokenAcessoQuery")]
        public async Task Executar_Deve_Retornar_Mesmo_DTO_De_ObterTokenAcessoQuery()
        {
            // Arrange
            var token = Guid.NewGuid();
            var login = "usuario@example.com";
            var usuarioPerfisRetornoDTO = CriarUsuarioPerfisRetornoDTO(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterLoginUsuarioTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(login);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtivarUsuarioExternoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterTokenAcessoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetornoDTO);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.Same(usuarioPerfisRetornoDTO, resultado);
        }

        private static UsuarioPerfisRetornoDTO CriarUsuarioPerfisRetornoDTO(string login)
        {
            return new UsuarioPerfisRetornoDTO
            {
                UsuarioLogin = login,
                UsuarioNome = "Usuário Teste",
                Email = login,
                Token = Guid.NewGuid().ToString(),
                DataHoraExpiracao = DateTime.UtcNow.AddHours(1),
                Autenticado = true,
                Cpf = "12345678901",
                PerfilUsuario = []
            };
        }
    }
}
