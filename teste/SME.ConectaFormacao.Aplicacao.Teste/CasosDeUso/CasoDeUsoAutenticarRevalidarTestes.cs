using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Autentiacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAutenticarRevalidarTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAutenticarRevalidar _casoDeUso;

        public CasoDeUsoAutenticarRevalidarTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoAutenticarRevalidar(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve revalidar token com sucesso quando token é válido")]
        public async Task Executar_Deve_Revalidar_Token_Com_Sucesso_Quando_Token_Valido()
        {
            // Arrange
            const string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvYW8gU2lsdmEiLCJpYXQiOjE1MTYyMzkwMjJ9";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(usuarioPerfisRetorno, resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve passar token correto para RevalidarTokenServicoAcessosQuery")]
        public async Task Executar_Deve_Passar_Token_Correto_Para_RevalidarTokenServicoAcessosQuery()
        {
            // Arrange
            const string tokenEsperado = "token_teste_123456";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            RevalidarTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UsuarioPerfisRetornoDTO>, CancellationToken>(
                    (query, ct) => queryCapturada = query as RevalidarTokenServicoAcessosQuery)
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(tokenEsperado);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(tokenEsperado, queryCapturada.Token);
        }

        [Fact(DisplayName = "Executar - Deve retornar UsuarioPerfisRetornoDTO com dados válidos")]
        public async Task Executar_Deve_Retornar_UsuarioPerfisRetornoDTO_Com_Dados_Validos()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<UsuarioPerfisRetornoDTO>(resultado);
            Assert.Equal(usuarioPerfisRetorno.UsuarioNome, resultado.UsuarioNome);
            Assert.Equal(usuarioPerfisRetorno.UsuarioLogin, resultado.UsuarioLogin);
            Assert.Equal(usuarioPerfisRetorno.Email, resultado.Email);
            Assert.Equal(usuarioPerfisRetorno.Cpf, resultado.Cpf);
            Assert.Equal(usuarioPerfisRetorno.Token, resultado.Token);
            Assert.Equal(usuarioPerfisRetorno.Autenticado, resultado.Autenticado);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var tarefa = _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<UsuarioPerfisRetornoDTO>>(tarefa);

            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken_Para_Mediator()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAutenticarRevalidar)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoAutenticarRevalidar deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoAutenticarRevalidar")]
        public void Executar_Deve_Implementar_Interface_ICasoDeUsoAutenticarRevalidar()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAutenticarRevalidar)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoAutenticarRevalidar"),
                "CasoDeUsoAutenticarRevalidar deve implementar ICasoDeUsoAutenticarRevalidar");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Com_IMediator()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoAutenticarRevalidar(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoAutenticarRevalidar>(casoDeUso);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoAutenticarRevalidar(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var campoMediator = typeof(CasoDeUsoAutenticarRevalidar)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
            Assert.Equal(mediatorMock.Object, valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com UsuarioNome preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_UsuarioNome_Preenchido()
        {
            // Arrange
            const string nomeUsuarioEsperado = "João Silva Oliveira";
            const string token = "token_teste";

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = nomeUsuarioEsperado,
                UsuarioLogin = "joao.silva",
                Token = token,
                Email = "joao@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(nomeUsuarioEsperado, resultado.UsuarioNome);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com UsuarioLogin preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_UsuarioLogin_Preenchido()
        {
            // Arrange
            const string loginEsperado = "usuario.login";
            const string token = "token_teste";

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário Teste",
                UsuarioLogin = loginEsperado,
                Token = token,
                Email = "usuario@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(loginEsperado, resultado.UsuarioLogin);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com Token preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Token_Preenchido()
        {
            // Arrange
            const string tokenRetornado = "novo_token_revalidado";
            const string tokenEnviado = "token_antigo";

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Teste",
                UsuarioLogin = "teste",
                Token = tokenRetornado,
                Email = "teste@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(tokenEnviado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tokenRetornado, resultado.Token);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com Email preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Email_Preenchido()
        {
            // Arrange
            const string emailEsperado = "usuario.revalidado@example.com.br";
            const string token = "token_teste";

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = emailEsperado,
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(emailEsperado, resultado.Email);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com Cpf preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Cpf_Preenchido()
        {
            // Arrange
            const string cpfEsperado = "12345678901";
            const string token = "token_teste";

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = "usuario@example.com",
                Cpf = cpfEsperado,
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(cpfEsperado, resultado.Cpf);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com Autenticado = true")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Autenticado_Verdadeiro()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = "usuario@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Autenticado);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com Autenticado = false")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Autenticado_Falso()
        {
            // Arrange
            const string token = "token_invalido";
            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = "usuario@example.com",
                Cpf = "12345678901",
                Autenticado = false,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Autenticado);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com DataHoraExpiracao preenchida")]
        public async Task Executar_Deve_Retornar_Resultado_Com_DataHoraExpiracao_Preenchida()
        {
            // Arrange
            const string token = "token_teste";
            var dataHoraExpiracaoEsperada = DateTime.UtcNow.AddHours(1);

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = "usuario@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                DataHoraExpiracao = dataHoraExpiracaoEsperada,
                PerfilUsuario = []
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(dataHoraExpiracaoEsperada, resultado.DataHoraExpiracao);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com PerfilUsuario preenchido")]
        public async Task Executar_Deve_Retornar_Resultado_Com_PerfilUsuario_Preenchido()
        {
            // Arrange
            const string token = "token_teste";
            var perfilUsuario = new List<PerfilUsuarioDTO>
            {
                new() { PerfilNome = "Administrador" },
                new() { PerfilNome = "Gestor" }
            };

            var usuarioPerfisRetorno = new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário",
                UsuarioLogin = "usuario",
                Token = token,
                Email = "usuario@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                PerfilUsuario = perfilUsuario
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var resultado = await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.PerfilUsuario);
            Assert.Equal(2, resultado.PerfilUsuario.Count);
        }

        [Fact(DisplayName = "Executar - Deve criar query com token não nulo")]
        public async Task Executar_Deve_Criar_Query_Com_Token_Nao_Nulo()
        {
            // Arrange
            const string token = "token_nao_nulo";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            RevalidarTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UsuarioPerfisRetornoDTO>, CancellationToken>(
                    (query, ct) => queryCapturada = query as RevalidarTokenServicoAcessosQuery)
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.NotNull(queryCapturada.Token);
            Assert.NotEmpty(queryCapturada.Token);
        }

        [Fact(DisplayName = "Executar - Deve retornar Task<UsuarioPerfisRetornoDTO>")]
        public async Task Executar_Deve_Retornar_Task_UsuarioPerfisRetornoDTO()
        {
            // Arrange
            const string token = "token_teste";
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            var tarefa = _casoDeUso.Executar(token);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<UsuarioPerfisRetornoDTO>>(tarefa);
        }

        [Fact(DisplayName = "Executar - Deve execução múltiplas vezes com tokens diferentes")]
        public async Task Executar_Deve_Executar_Multiplas_Vezes_Com_Tokens_Diferentes()
        {
            // Arrange
            const string token1 = "token_um";
            const string token2 = "token_dois";
            const string token3 = "token_tres";

            var usuarioPerfisRetorno1 = CriarUsuarioPerfisRetornoDTOValido(token1);
            var usuarioPerfisRetorno2 = CriarUsuarioPerfisRetornoDTOValido(token2);
            var usuarioPerfisRetorno3 = CriarUsuarioPerfisRetornoDTOValido(token3);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((RevalidarTokenServicoAcessosQuery query, CancellationToken ct) =>
                    query.Token switch
                    {
                        "token_um" => usuarioPerfisRetorno1,
                        "token_dois" => usuarioPerfisRetorno2,
                        "token_tres" => usuarioPerfisRetorno3,
                        _ => CriarUsuarioPerfisRetornoDTOValido()
                    });

            // Act
            var resultado1 = await _casoDeUso.Executar(token1);
            var resultado2 = await _casoDeUso.Executar(token2);
            var resultado3 = await _casoDeUso.Executar(token3);

            // Assert
            Assert.NotNull(resultado1);
            Assert.NotNull(resultado2);
            Assert.NotNull(resultado3);
            Assert.Equal(token1, resultado1.Token);
            Assert.Equal(token2, resultado2.Token);
            Assert.Equal(token3, resultado3.Token);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        [Fact(DisplayName = "Executar - Deve aceitar token com caracteres especiais")]
        public async Task Executar_Deve_Aceitar_Token_Com_Caracteres_Especiais()
        {
            // Arrange
            const string tokenComCaracteresEspeciais = "token_teste-123.456!@#$%^&*()_+=[]{}|;':\"\\,.<>?/";

            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            RevalidarTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UsuarioPerfisRetornoDTO>, CancellationToken>(
                    (query, ct) => queryCapturada = query as RevalidarTokenServicoAcessosQuery)
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(tokenComCaracteresEspeciais);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(tokenComCaracteresEspeciais, queryCapturada.Token);
        }

        [Fact(DisplayName = "Executar - Deve aceitar token muito longo")]
        public async Task Executar_Deve_Aceitar_Token_Muito_Longo()
        {
            // Arrange
            var tokenMuitoLongo = new string('a', 10000);
            var usuarioPerfisRetorno = CriarUsuarioPerfisRetornoDTOValido();

            RevalidarTokenServicoAcessosQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RevalidarTokenServicoAcessosQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UsuarioPerfisRetornoDTO>, CancellationToken>(
                    (query, ct) => queryCapturada = query as RevalidarTokenServicoAcessosQuery)
                .ReturnsAsync(usuarioPerfisRetorno);

            // Act
            await _casoDeUso.Executar(tokenMuitoLongo);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(tokenMuitoLongo, queryCapturada.Token);
        }

        private static UsuarioPerfisRetornoDTO CriarUsuarioPerfisRetornoDTOValido(string? token = null)
        {
            return new UsuarioPerfisRetornoDTO
            {
                UsuarioNome = "Usuário Teste",
                UsuarioLogin = "usuario.teste",
                Token = token ?? "token_padrao",
                Email = "usuario.teste@example.com",
                Cpf = "12345678901",
                Autenticado = true,
                DataHoraExpiracao = DateTime.UtcNow.AddHours(1),
                PerfilUsuario = []
            };
        }
    }
}
