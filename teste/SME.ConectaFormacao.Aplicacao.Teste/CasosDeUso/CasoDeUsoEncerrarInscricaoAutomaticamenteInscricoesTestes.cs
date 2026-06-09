using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes _casoDeUso;

        public CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoesTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar true quando mensagem é nula")]
        public async Task Executar_Deve_Retornar_True_Quando_Mensagem_Nula()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit();

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact(DisplayName = "Executar - Deve obter inscrições quando turmaId é válido")]
        public async Task Executar_Deve_Obter_Inscricoes_Quando_TurmaId_Valido()
        {
            // Arrange
            const long turmaId = 123;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 1, UsuarioId = 100 },
                new() { InscricaoId = 2, UsuarioId = 101 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve publicar na fila quando inscrições existem")]
        public async Task Executar_Deve_Publicar_Na_Fila_Quando_Inscricoes_Existem()
        {
            // Arrange
            const long turmaId = 456;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 10, UsuarioId = 200 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Não deve publicar na fila quando não há inscrições")]
        public async Task Executar_Nao_Deve_Publicar_Na_Fila_Quando_Sem_Inscricoes()
        {
            // Arrange
            const long turmaId = 789;
            var inscricoes = new List<InscricaoUsuarioInternoDto>();

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact(DisplayName = "Executar - Deve retornar true sempre")]
        public async Task Executar_Deve_Retornar_True_Sempre()
        {
            // Arrange
            const long turmaId = 999;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

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
            const long turmaId = 111;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            var tarefa = _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(tarefa);
            Assert.IsType<Task<bool>>(tarefa);
            await tarefa;
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes"),
                "CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes deve implementar ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com ObtertInscricoesPorPropostaTurmaQuery")]
        public async Task Executar_Deve_Chamar_Mediator_Com_Query()
        {
            // Arrange
            const long turmaId = 222;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObtertInscricoesPorPropostaTurmaQuery>(q => q.TurmasIds.Contains(turmaId)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve passar turmaId correto na query")]
        public async Task Executar_Deve_Passar_TurmaId_Correto_Na_Query()
        {
            // Arrange
            const long turmaIdEsperado = 333;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaIdEsperado);

            ObtertInscricoesPorPropostaTurmaQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<InscricaoUsuarioInternoDto>>, CancellationToken>
                (
                    (query, ct) => queryCapturada = query as ObtertInscricoesPorPropostaTurmaQuery
                )
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.NotNull(queryCapturada.TurmasIds);
            Assert.Single(queryCapturada.TurmasIds);
            Assert.Equal(turmaIdEsperado, queryCapturada.TurmasIds[0]);
        }

        [Fact(DisplayName = "Executar - Deve usar rota EncerrarInscricaoAutomaticamenteUsuarios na publicação")]
        public async Task Executar_Deve_Usar_Rota_Correta_Na_Publicacao()
        {
            // Arrange
            const long turmaId = 444;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 50, UsuarioId = 300 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            PublicarNaFilaRabbitCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as PublicarNaFilaRabbitCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios, commandCapturado.Rota);
        }

        [Fact(DisplayName = "Executar - Deve passar inscrições como filtros na publicação")]
        public async Task Executar_Deve_Passar_Inscricoes_Como_Filtros()
        {
            // Arrange
            const long turmaId = 555;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 100, UsuarioId = 400 },
                new() { InscricaoId = 101, UsuarioId = 401 },
                new() { InscricaoId = 102, UsuarioId = 402 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            PublicarNaFilaRabbitCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as PublicarNaFilaRabbitCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(inscricoes, commandCapturado.Filtros);
        }

        [Fact(DisplayName = "Executar - Deve gerar novo Guid como CodigoCorrelacao")]
        public async Task Executar_Deve_Gerar_Novo_Guid_CodigoCorrelacao()
        {
            // Arrange
            const long turmaId = 666;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 200, UsuarioId = 500 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var guidsCapturaos = new List<Guid>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        var cmd = command as PublicarNaFilaRabbitCommand;
                        if (cmd != null)
                            guidsCapturaos.Add(cmd.CodigoCorrelacao);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(2, guidsCapturaos.Count);
            Assert.NotEqual(guidsCapturaos[0], guidsCapturaos[1]);
        }

        [Fact(DisplayName = "Executar - Deve criar usuário 'Sistema' para publicação")]
        public async Task Executar_Deve_Criar_Usuario_Sistema_Para_Publicacao()
        {
            // Arrange
            const long turmaId = 777;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 300, UsuarioId = 600 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            PublicarNaFilaRabbitCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as PublicarNaFilaRabbitCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.NotNull(commandCapturado.Usuario);
            Assert.Equal("Sistema", commandCapturado.Usuario.Nome);
            Assert.Equal("Sistema", commandCapturado.Usuario.Login);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com CancellationToken")]
        public async Task Executar_Deve_Chamar_Mediator_Com_CancellationToken()
        {
            // Arrange
            const long turmaId = 888;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com múltiplas inscrições")]
        public async Task Executar_Deve_Funcionar_Com_Multiplas_Inscricoes()
        {
            // Arrange
            const long turmaId = 1000;
            var inscricoes = Enumerable.Range(1, 100)
                .Select(i => new InscricaoUsuarioInternoDto { InscricaoId = i, UsuarioId = 1000 + i })
                .ToList();

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve deserializar turmaId como long")]
        public async Task Executar_Deve_Deserializar_TurmaId_Como_Long()
        {
            // Arrange
            const long turmaIdEsperado = 9999999;
            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaIdEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObtertInscricoesPorPropostaTurmaQuery>(
                        q => q.TurmasIds.Contains(turmaIdEsperado)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve manter ordem de execução: Query -> Publish")]
        public async Task Executar_Deve_Manter_Ordem_Execucao()
        {
            // Arrange
            const long turmaId = 1111;
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new() { InscricaoId = 400, UsuarioId = 700 }
            };

            var mensagemRabbit = CriarMensagemRabbitComTurmaId(turmaId);
            var ordemExecucao = new List<string>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObtertInscricoesPorPropostaTurmaQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<InscricaoUsuarioInternoDto>>, CancellationToken>(
                    (query, ct) => ordemExecucao.Add("Query"))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => ordemExecucao.Add("Publish"))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(2, ordemExecucao.Count);
            Assert.Equal("Query", ordemExecucao[0]);
            Assert.Equal("Publish", ordemExecucao[1]);
        }

        private static MensagemRabbit CriarMensagemRabbitComTurmaId(long turmaId)
        {
            var json = turmaId.ObjetoParaJson();
            return new MensagemRabbit(json);
        }
    }
}
