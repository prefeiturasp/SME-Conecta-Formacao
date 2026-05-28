using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEncerrarInscricaoCursistaInativoSemCargoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo _casoDeUso;

        public CasoDeUsoEncerrarInscricaoCursistaInativoSemCargoTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve obter propostas confirmadas que não encerraram ainda")]
        public async Task Executar_Deve_Obter_Propostas_Confirmadas_Que_Nao_Encerraram_Ainda()
        {
            // Arrange
            var propostasIds = new[] { 1L, 2L, 3L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve publicar mensagem para cada proposta")]
        public async Task Executar_Deve_Publicar_Mensagem_Para_Cada_Proposta()
        {
            // Arrange
            var propostasIds = new[] { 10L, 20L, 30L, 40L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
                Times.Exactly(propostasIds.Length));
        }

        [Fact(DisplayName = "Executar - Deve publicar com rota EncerrarInscricaoAutomaticamenteTurma")]
        public async Task Executar_Deve_Publicar_Com_Rota_Correta()
        {
            // Arrange
            var propostasIds = new[] { 50L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
            Assert.Equal(RotasRabbit.EncerrarInscricaoAutomaticamenteTurma, commandCapturado.Rota);
        }

        [Fact(DisplayName = "Executar - Deve passar propostaId como filtro na publicação")]
        public async Task Executar_Deve_Passar_PropostaId_Como_Filtro()
        {
            // Arrange
            const long propostaIdEsperada = 999;
            var propostasIds = new[] { propostaIdEsperada };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
            Assert.Equal(propostaIdEsperada, commandCapturado.Filtros);
        }

        [Fact(DisplayName = "Executar - Deve gerar novo Guid como CodigoCorrelacao para cada proposta")]
        public async Task Executar_Deve_Gerar_Novo_Guid_Para_Cada_Proposta()
        {
            // Arrange
            var propostasIds = new[] { 100L, 200L, 300L };
            var mensagemRabbit = new MensagemRabbit();

            var guidsCapturaos = new List<Guid>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
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
            var propostasIds = new[] { 400L, 500L };
            var mensagemRabbit = new MensagemRabbit();

            var usuariosCapturados = new List<Dominio.Entidades.Usuario>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
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
            var propostasIds = new[] { 600L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
            var propostasIds = new[] { 700L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
                typeof(CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo"),
                "CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo deve implementar ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoEncerrarInscricaoCursistaInativoSemCargo(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoEncerrarInscricaoCursistaInativoSemCargo>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com CancellationToken")]
        public async Task Executar_Deve_Chamar_Mediator_Com_CancellationToken()
        {
            // Arrange
            var propostasIds = new[] { 800L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com múltiplas propostas")]
        public async Task Executar_Deve_Funcionar_Com_Multiplas_Propostas()
        {
            // Arrange
            var propostasIds = Enumerable.Range(1, 50)
                .Select(i => (long)i)
                .ToArray();

            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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

        [Fact(DisplayName = "Executar - Deve não chamar PublicarNaFilaRabbitCommand quando lista de propostas vazia")]
        public async Task Executar_Deve_Nao_Chamar_Publish_Quando_Sem_Propostas()
        {
            // Arrange
            var propostasIds = Array.Empty<long>();
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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

        [Fact(DisplayName = "Executar - Deve manter ordem de execução: Query -> Loop de Publish")]
        public async Task Executar_Deve_Manter_Ordem_Execucao()
        {
            // Arrange
            var propostasIds = new[] { 900L, 1000L };
            var mensagemRabbit = new MensagemRabbit();
            var ordemExecucao = new List<string>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<long>>, CancellationToken>(
                    (query, ct) => ordemExecucao.Add("Query"))
                .ReturnsAsync(propostasIds);

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

        [Fact(DisplayName = "Executar - Deve iterar sobre todas as propostas")]
        public async Task Executar_Deve_Iterar_Sobre_Todas_As_Propostas()
        {
            // Arrange
            const long propostaId1 = 1100;
            const long propostaId2 = 1200;
            const long propostaId3 = 1300;
            var propostasIds = new[] { propostaId1, propostaId2, propostaId3 };
            var mensagemRabbit = new MensagemRabbit();

            var propostasEnviadas = new List<long>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
                            propostasEnviadas.Add((long)cmd.Filtros!);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(3, propostasEnviadas.Count);
            Assert.Contains(propostaId1, propostasEnviadas);
            Assert.Contains(propostaId2, propostasEnviadas);
            Assert.Contains(propostaId3, propostasEnviadas);
        }

        [Fact(DisplayName = "Executar - Deve capturar corretamente filtro de cada proposta no publish")]
        public async Task Executar_Deve_Capturar_Filtro_Corretamente()
        {
            // Arrange
            var propostasIds = new[] { 1400L, 1500L, 1600L };
            var mensagemRabbit = new MensagemRabbit();

            var filtrosCapturaos = new List<long>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd && cmd.Filtros is long filtro)
                            filtrosCapturaos.Add(filtro);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            Assert.Equal(propostasIds, filtrosCapturaos);
        }

        [Fact(DisplayName = "Executar - Deve passar MensagemRabbit como parâmetro")]
        public async Task Executar_Deve_Aceitar_MensagemRabbit_Como_Parametro()
        {
            // Arrange
            var propostasIds = new[] { 1700L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await _casoDeUso.Executar(mensagemRabbit);

            _mediatorMock.Verify(m => m.Send(
                It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve verificar que PropostasConfirmadasQueNaoEncerramAindaQuery é chamada uma única vez")]
        public async Task Executar_Deve_Chamar_Query_Uma_Unica_Vez()
        {
            // Arrange
            var propostasIds = new[] { 1800L, 1900L, 2000L };
            var mensagemRabbit = new MensagemRabbit();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostasIds);

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
                    It.IsAny<PropostasConfirmadasQueNaoEncerramAindaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
