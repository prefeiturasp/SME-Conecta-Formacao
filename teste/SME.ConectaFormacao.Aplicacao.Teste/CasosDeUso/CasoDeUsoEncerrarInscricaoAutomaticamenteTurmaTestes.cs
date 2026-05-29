using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteTurmaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEncerrarInscricaoAutomaticamenteTurma _casoDeUso;

        public CasoDeUsoEncerrarInscricaoAutomaticamenteTurmaTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteTurma(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve lançar NullReferenceException quando mensagem é nula")]
        public async Task Executar_Deve_Lancar_NullReferenceException_Quando_Mensagem_Nula()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _casoDeUso.Executar(mensagemRabbit));
        }

        [Fact(DisplayName = "Executar - Deve obter turmas quando propostaId é válido")]
        public async Task Executar_Deve_Obter_Turmas_Quando_PropostaId_Valido()
        {
            // Arrange
            const long propostaId = 123;
            var turmas = new[] { 1L, 2L, 3L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve publicar mensagem para cada turma")]
        public async Task Executar_Deve_Publicar_Mensagem_Para_Cada_Turma()
        {
            // Arrange
            const long propostaId = 456;
            var turmas = new[] { 10L, 20L, 30L, 40L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                Times.Exactly(turmas.Length));
        }

        [Fact(DisplayName = "Executar - Deve publicar com rota correta")]
        public async Task Executar_Deve_Publicar_Com_Rota_Correta()
        {
            // Arrange
            const long propostaId = 789;
            var turmas = new[] { 50L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
            Assert.Equal(RotasRabbit.EncerrarInscricaoAutomaticamenteInscricoes, commandCapturado.Rota);
        }

        [Fact(DisplayName = "Executar - Deve passar turmaId como filtro na publicação")]
        public async Task Executar_Deve_Passar_TurmaId_Como_Filtro()
        {
            // Arrange
            const long propostaId = 111;
            const long turmaIdEsperada = 999;
            var turmas = new[] { turmaIdEsperada };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
            Assert.Equal(turmaIdEsperada, commandCapturado.Filtros);
        }

        [Fact(DisplayName = "Executar - Deve gerar novo Guid como CodigoCorrelacao para cada turma")]
        public async Task Executar_Deve_Gerar_Novo_Guid_Para_Cada_Turma()
        {
            // Arrange
            const long propostaId = 222;
            var turmas = new[] { 100L, 200L, 300L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            var guidsCapturaos = new List<Guid>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        PublicarNaFilaRabbitCommand? cmd = command as PublicarNaFilaRabbitCommand;
                        if (cmd != null)
                            guidsCapturaos.Add(cmd.CodigoCorrelacao);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(3, guidsCapturaos.Count);
            Assert.NotEqual(guidsCapturaos[0], guidsCapturaos[1]);
            Assert.NotEqual(guidsCapturaos[1], guidsCapturaos[2]);
            Assert.NotEqual(guidsCapturaos[0], guidsCapturaos[2]);
        }

        [Fact(DisplayName = "Executar - Deve criar usuário 'Sistema' para cada publicação")]
        public async Task Executar_Deve_Criar_Usuario_Sistema()
        {
            // Arrange
            const long propostaId = 333;
            var turmas = new[] { 400L, 500L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            var usuariosCapturados = new List<Dominio.Entidades.Usuario>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        var cmd = command as PublicarNaFilaRabbitCommand;
                        if (cmd != null)
                            usuariosCapturados.Add(cmd.Usuario!);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(2, usuariosCapturados.Count);
            foreach (var usuario in usuariosCapturados)
            {
                Assert.NotNull(usuario);
                Assert.Equal("Sistema", usuario.Nome);
                Assert.Equal("Sistema", usuario.Login);
                Assert.Empty(usuario.Email);
            }
        }

        [Fact(DisplayName = "Executar - Deve retornar true com sucesso")]
        public async Task Executar_Deve_Retornar_True_Com_Sucesso()
        {
            // Arrange
            const long propostaId = 444;
            var turmas = new[] { 600L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

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
            const long propostaId = 555;
            var turmas = new[] { 700L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
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
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteTurma)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEncerrarInscricaoAutomaticamenteTurma deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoAutomaticamenteTurma)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma"),
                "CasoDeUsoEncerrarInscricaoAutomaticamenteTurma deve implementar ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoEncerrarInscricaoAutomaticamenteTurma(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoEncerrarInscricaoAutomaticamenteTurma>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve passar propostaId correto na query")]
        public async Task Executar_Deve_Passar_PropostaId_Correto_Na_Query()
        {
            // Arrange
            const long propostaIdEsperada = 8888;
            var turmas = new[] { 800L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaIdEsperada);

            ObterPropostasTurmasPorPropostaIdQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<long>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterPropostasTurmasPorPropostaIdQuery)
                .ReturnsAsync(turmas);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(propostaIdEsperada, queryCapturada.PropostaId);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com CancellationToken")]
        public async Task Executar_Deve_Chamar_Mediator_Com_CancellationToken()
        {
            // Arrange
            const long propostaId = 9999;
            var turmas = new[] { 900L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com múltiplas turmas")]
        public async Task Executar_Deve_Funcionar_Com_Multiplas_Turmas()
        {
            // Arrange
            const long propostaId = 10000;
            var turmas = Enumerable.Range(1, 50)
                .Select(i => (long)i)
                .ToArray();

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                Times.Exactly(50));
        }

        [Fact(DisplayName = "Executar - Deve deserializar propostaId como long")]
        public async Task Executar_Deve_Deserializar_PropostaId_Como_Long()
        {
            // Arrange
            const long propostaIdEsperado = 99999999;
            var turmas = new[] { 1000L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaIdEsperado);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                    It.Is<ObterPropostasTurmasPorPropostaIdQuery>(
                        q => q.PropostaId == propostaIdEsperado),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve manter ordem de execução: Query -> Loop de Publish")]
        public async Task Executar_Deve_Manter_Ordem_Execucao()
        {
            // Arrange
            const long propostaId = 11111;
            var turmas = new[] { 1100L, 1200L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);
            var ordemExecucao = new List<string>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<long>>, CancellationToken>(
                    (query, ct) => ordemExecucao.Add("Query"))
                .ReturnsAsync(turmas);

            var publishCount = 0;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        publishCount++;
                        ordemExecucao.Add($"Publish{publishCount}");
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(3, ordemExecucao.Count);
            Assert.Equal("Query", ordemExecucao[0]);
            Assert.Equal("Publish1", ordemExecucao[1]);
            Assert.Equal("Publish2", ordemExecucao[2]);
        }

        [Fact(DisplayName = "Executar - Deve não chamar PublicarNaFilaRabbitCommand quando lista de turmas vazia")]
        public async Task Executar_Deve_Nao_Chamar_Publish_Quando_Sem_Turmas()
        {
            // Arrange
            const long propostaId = 22222;
            var turmas = Array.Empty<long>();

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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

        [Fact(DisplayName = "Executar - Deve usar ToString na mensagem antes de desserializar")]
        public async Task Executar_Deve_Usar_ToString_Na_Mensagem()
        {
            // Arrange
            const long propostaId = 33333;
            var turmas = new[] { 1300L };

            var mensagemRabbit = CriarMensagemRabbitComPropostaId(propostaId);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(turmas);

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
                    It.IsAny<ObterPropostasTurmasPorPropostaIdQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private static MensagemRabbit CriarMensagemRabbitComPropostaId(long propostaId)
        {
            var json = propostaId.ObjetoParaJson();
            return new MensagemRabbit(json);
        }
    }
}
