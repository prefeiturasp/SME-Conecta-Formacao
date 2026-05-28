using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteUsuariosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios _casoDeUso;

        public CasoDeUsoEncerrarInscricaoAutomaticamenteUsuariosTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve obter usuários inscrição da mensagem")]
        public async Task Executar_Deve_Obter_Usuarios_Inscricao_Da_Mensagem()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1, UsuarioId = 100, Login = "user1" },
                new() { InscricaoId = 2, UsuarioId = 101, Login = "user2" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["user1", "user2"]);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve verificar usuários ativos no EOL")]
        public async Task Executar_Deve_Verificar_Usuarios_Ativos_No_Eol()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 10, UsuarioId = 200, Login = "user10" },
                new() { InscricaoId = 11, UsuarioId = 201, Login = "user11" },
                new() { InscricaoId = 12, UsuarioId = 202, Login = "user12" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            VerificarSeUsuarioPossuiCargoAtivoNoEolQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<string>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as VerificarSeUsuarioPossuiCargoAtivoNoEolQuery)
                .ReturnsAsync(["user10", "user11", "user12"]);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(3, queryCapturada.Login.Length);
            Assert.Contains("user10", queryCapturada.Login);
            Assert.Contains("user11", queryCapturada.Login);
            Assert.Contains("user12", queryCapturada.Login);
        }

        [Fact(DisplayName = "Executar - Deve cancelar inscrições de usuários inativos")]
        public async Task Executar_Deve_Cancelar_Inscricoes_Usuarios_Inativos()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 100, UsuarioId = 1000, Login = "ativo" },
                new() { InscricaoId = 101, UsuarioId = 1001, Login = "inativo" },
                new() { InscricaoId = 102, UsuarioId = 1002, Login = "ativo2" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["ativo", "ativo2"]);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<CancelarInscricaoCommand>(c => c.Id == 101),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar CancelarInscricaoCommand apenas para usuários inativos")]
        public async Task Executar_Deve_Chamar_CancelarInscricaoCommand_Apenas_Para_Inativos()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 200, UsuarioId = 2000, Login = "ativo" },
                new() { InscricaoId = 201, UsuarioId = 2001, Login = "inativo1" },
                new() { InscricaoId = 202, UsuarioId = 2002, Login = "inativo2" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["ativo"]);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact(DisplayName = "Executar - Deve retornar true após executar")]
        public async Task Executar_Deve_Retornar_True()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 300, UsuarioId = 3000, Login = "user300" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["user300"]);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            Assert.IsType<bool>(resultado);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 400, UsuarioId = 4000, Login = "user400" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["user400"]);

            // Act
            var tarefa = _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<bool>>(tarefa);
            await tarefa;
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios"),
                "Deve implementar ICasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteUsuarios(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
        }

        [Fact(DisplayName = "Executar - Deve passar login corretamente na query")]
        public async Task Executar_Deve_Passar_Login_Correto_Na_Query()
        {
            // Arrange
            const string loginEsperado1 = "user_especifico1";
            const string loginEsperado2 = "user_especifico2";
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 500, UsuarioId = 5000, Login = loginEsperado1 },
                new() { InscricaoId = 501, UsuarioId = 5001, Login = loginEsperado2 }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            VerificarSeUsuarioPossuiCargoAtivoNoEolQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<string>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as VerificarSeUsuarioPossuiCargoAtivoNoEolQuery)
                .ReturnsAsync([loginEsperado1, loginEsperado2]);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Contains(loginEsperado1, queryCapturada.Login);
            Assert.Contains(loginEsperado2, queryCapturada.Login);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com CancellationToken")]
        public async Task Executar_Deve_Chamar_Mediator_Com_CancellationToken()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 600, UsuarioId = 6000, Login = "user600" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["user600"]);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com múltiplos usuários")]
        public async Task Executar_Deve_Funcionar_Com_Multiplos_Usuarios()
        {
            // Arrange
            var usuariosInscricao = Enumerable.Range(1, 50)
                .Select(i => new InscricaoUsuarioInternoDto
                {
                    InscricaoId = i,
                    UsuarioId = 7000 + i,
                    Login = $"user{7000 + i}"
                })
                .ToList();

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([.. usuariosInscricao.Take(25).Select(u => u.Login)]);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(25));
        }

        [Fact(DisplayName = "Executar - Deve manter ordem de execução")]
        public async Task Executar_Deve_Manter_Ordem_Execucao()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 800, UsuarioId = 8000, Login = "user800" },
                new() { InscricaoId = 801, UsuarioId = 8001, Login = "user801" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);
            var ordemExecucao = new List<string>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<string>>, CancellationToken>(
                    (query, ct) => ordemExecucao.Add("VerificarAtivos"))
                .ReturnsAsync(["user800"]);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => ordemExecucao.Add("Cancelar"))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(ordemExecucao.Count >= 1);
            Assert.Equal("VerificarAtivos", ordemExecucao[0]);
        }

        [Fact(DisplayName = "Executar - Não deve cancelar inscrições quando todos usuários estão ativos")]
        public async Task Executar_Nao_Deve_Cancelar_Quando_Todos_Ativos()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 900, UsuarioId = 9000, Login = "user900" },
                new() { InscricaoId = 901, UsuarioId = 9001, Login = "user901" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(["user900", "user901"]);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact(DisplayName = "Executar - Deve passar null como motivo no CancelarInscricaoCommand")]
        public async Task Executar_Deve_Passar_Null_Como_Motivo()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1000, UsuarioId = 10000, Login = "user1000" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            CancelarInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as CancelarInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Null(commandCapturado.Motivo);
        }

        [Fact(DisplayName = "Executar - Deve filtrar corretamente usuários inativos")]
        public async Task Executar_Deve_Filtrar_Usuarios_Inativos()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1100, UsuarioId = 11000, Login = "ativo1" },
                new() { InscricaoId = 1101, UsuarioId = 11001, Login = "inativo1" },
                new() { InscricaoId = 1102, UsuarioId = 11002, Login = "ativo2" },
                new() { InscricaoId = 1103, UsuarioId = 11003, Login = "inativo2" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);
            var usuariosAtivos = new List<string> { "ativo1", "ativo2" };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuariosAtivos);

            var inscricoesCanceladas = new List<long>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is CancelarInscricaoCommand cmd)
                            inscricoesCanceladas.Add(cmd.Id);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(2, inscricoesCanceladas.Count);
            Assert.Contains(1101, inscricoesCanceladas);
            Assert.Contains(1103, inscricoesCanceladas);
            Assert.DoesNotContain(1100, inscricoesCanceladas);
            Assert.DoesNotContain(1102, inscricoesCanceladas);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado correto após cancelamentos")]
        public async Task Executar_Deve_Retornar_Resultado_Correto()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1200, UsuarioId = 12000, Login = "user1200" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve usar Where para filtrar inativos")]
        public async Task Executar_Deve_Usar_Where_Para_Filtrar()
        {
            // Arrange
            var usuariosInscricao = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1300, UsuarioId = 13000, Login = "login1" },
                new() { InscricaoId = 1301, UsuarioId = 13001, Login = "login2" },
                new() { InscricaoId = 1302, UsuarioId = 13002, Login = "login3" },
                new() { InscricaoId = 1303, UsuarioId = 13003, Login = "login4" }
            };

            var mensagemRabbit = CriarMensagemRabbit(usuariosInscricao);
            var usuariosAtivos = new List<string> { "login1", "login3" };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuariosAtivos);

            var idsEnviados = new List<long>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CancelarInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is CancelarInscricaoCommand cmd)
                            idsEnviados.Add(cmd.Id);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(2, idsEnviados.Count);
            Assert.Equal(1301, idsEnviados[0]);
            Assert.Equal(1303, idsEnviados[1]);
        }

        private static MensagemRabbit CriarMensagemRabbit(IEnumerable<InscricaoUsuarioInternoDto> usuarios)
        {
            var json = usuarios.ObjetoParaJson();
            return new MensagemRabbit(json);
        }
    }
}
