using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista _casoDeUso;

        public CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista(_mediatorMock.Object);
        }

        #region Testes Comportamentais Positivos

        [Fact(DisplayName = "Executar - Deve retornar true quando há inscrições a processar")]
        public async Task Executar_Deve_Retornar_True_Quando_Ha_Inscricoes()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = "user1" }),
                CriarInscricao(2, "CARGO001", null, new Usuario { Login = "user2" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar false quando não há inscrições a processar")]
        public async Task Executar_Deve_Retornar_False_Quando_Sem_Inscricoes()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Executar - Deve publicar mensagem na fila para cada inscrição com cargo vazio")]
        public async Task Executar_Deve_Publicar_Mensagem_Para_Cada_Inscricao_Com_Cargo_Vazio()
        {
            // Arrange
            const int quantidadeInscricoes = 3;
            var inscricoes = Enumerable.Range(1, quantidadeInscricoes)
                .Select(i => CriarInscricao(i, null!, null, new Usuario { Login = $"user{i}" }))
                .ToList();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(quantidadeInscricoes));
        }

        [Fact(DisplayName = "Executar - Deve publicar mensagem na fila para cada inscrição com TipoVinculo nulo")]
        public async Task Executar_Deve_Publicar_Mensagem_Para_Cada_Inscricao_Com_TipoVinculo_Nulo()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, "CARGO001", null, new Usuario { Login = "user1" }),
                CriarInscricao(2, "CARGO002", null, new Usuario { Login = "user2" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact(DisplayName = "Executar - Deve publicar mensagem na fila para cada inscrição com TipoVinculo zero")]
        public async Task Executar_Deve_Publicar_Mensagem_Para_Cada_Inscricao_Com_TipoVinculo_Zero()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, "CARGO001", 0, new Usuario { Login = "user1" }),
                CriarInscricao(2, "CARGO002", 0, new Usuario { Login = "user2" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact(DisplayName = "Executar - Não deve publicar mensagem para inscrição com cargo preenchido e TipoVinculo válido")]
        public async Task Executar_Nao_Deve_Publicar_Mensagem_Para_Inscricao_Valida()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, "CARGO001", 1, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.False(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact(DisplayName = "Executar - Deve filtrar corretamente inscrições com cargo nulo ou vazio")]
        public async Task Executar_Deve_Filtrar_Corretamente_Inscricoes_Com_Cargo_Nulo_Ou_Vazio()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, 1, new Usuario { Login = "user1" }),      // cargo null - deve processar
                CriarInscricao(2, string.Empty, 1, new Usuario { Login = "user2" }), // cargo vazio - deve processar
                CriarInscricao(3, "CARGO003", 1, new Usuario { Login = "user3" })  // cargo preenchido - não deve processar
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        #endregion

        #region Testes de Mapeamento e Dados

        [Fact(DisplayName = "Executar - Deve passar dados corretos para PublicarNaFilaRabbitCommand")]
        public async Task Executar_Deve_Passar_Dados_Corretos_Para_PublicarNaFilaRabbitCommand()
        {
            // Arrange
            const long inscricaoId = 123;
            const string login = "usuario.teste";

            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(inscricaoId, null!, null, new Usuario { Login = login })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
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
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursistaTratar, commandCapturado.Rota);
        }

        [Fact(DisplayName = "Executar - Deve mapear ID da inscrição corretamente")]
        public async Task Executar_Deve_Mapear_Id_Da_Inscricao_Corretamente()
        {
            // Arrange
            const long inscricaoId = 456;
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(inscricaoId, null!, null, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var comandosCapturados = new List<PublicarNaFilaRabbitCommand>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
                            comandosCapturados.Add(cmd);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.Single(comandosCapturados);
            // Verificar que o DTO interno contém o ID correto
            Assert.NotNull(comandosCapturados[0]);
        }

        [Fact(DisplayName = "Executar - Deve mapear Login do usuário corretamente")]
        public async Task Executar_Deve_Mapear_Login_Usuario_Corretamente()
        {
            // Arrange
            const string loginEsperado = "rf.usuario.teste";
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = loginEsperado })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var comandosCapturados = new List<PublicarNaFilaRabbitCommand>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                 .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
                            comandosCapturados.Add(cmd);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.Single(comandosCapturados);
            Assert.NotNull(comandosCapturados[0]);
        }

        [Fact(DisplayName = "Executar - Deve mapear CargoCodigo da inscrição corretamente")]
        public async Task Executar_Deve_Mapear_CargoCodigo_Corretamente()
        {
            // Arrange
            const string cargoCodigoEsperado = "CARGO_EXEMPLO";
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, cargoCodigoEsperado, null, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var comandosCapturados = new List<PublicarNaFilaRabbitCommand>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                  .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
                            comandosCapturados.Add(cmd);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.Single(comandosCapturados);
            Assert.NotNull(comandosCapturados[0]);
        }

        [Fact(DisplayName = "Executar - Deve processar múltiplas inscrições com dados diferentes")]
        public async Task Executar_Deve_Processar_Multiplas_Inscricoes_Com_Dados_Diferentes()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = "user1" }),
                CriarInscricao(2, "CARGO_B", null, new Usuario { Login = "user2" }),
                CriarInscricao(3, null!, 0, new Usuario { Login = "user3" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var comandosCapturados = new List<PublicarNaFilaRabbitCommand>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        if (command is PublicarNaFilaRabbitCommand cmd)
                            comandosCapturados.Add(cmd);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.Equal(3, comandosCapturados.Count);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(3));
        }

        #endregion

        #region Testes de Estrutura e Interface

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista"),
                "CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista deve implementar ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursista>(casoDeUso, exactMatch: false);
        }

        #endregion

        #region Testes de Chamadas Mediator

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com ObterInscricoesConfirmadasQuery")]
        public async Task Executar_Deve_Chamar_Mediator_Com_Query()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez por inscrição a processar")]
        public async Task Executar_Deve_Chamar_Mediator_Uma_Vez_Por_Inscricao()
        {
            // Arrange
            const int quantidadeInscricoes = 5;
            var inscricoes = Enumerable.Range(1, quantidadeInscricoes)
                .Select(i => CriarInscricao(i, null!, null, new Usuario { Login = $"user{i}" }))
                .ToList();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(quantidadeInscricoes));
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para todas as chamadas mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken_Para_Todas_Chamadas()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<IRequest<IEnumerable<Inscricao>>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Testes de Assincronia

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var tarefa = _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<bool>>(tarefa);

            var resultado = await tarefa;
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Executar - Deve executar de forma assíncrona sem bloqueio")]
        public async Task Executar_Deve_Executar_De_Forma_Assincrona_Sem_Bloqueio()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = "user1" })
            };

            var tarefaCompleta = new TaskCompletionSource<bool>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IEnumerable<Inscricao>>(inscricoes));

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region Testes de Casos Limites

        [Fact(DisplayName = "Executar - Deve lidar com inscrições com login nulo graciosamente")]
        public async Task Executar_Deve_Lidar_Com_Login_Nulo()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = null! })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve lidar com inscrições com cargo vazio corretamente")]
        public async Task Executar_Deve_Lidar_Com_Cargo_Vazio()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, string.Empty, 1, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve lidar com inscrições com ID 0")]
        public async Task Executar_Deve_Lidar_Com_Inscricao_Id_Zero()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(0, null!, null, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve lidar com grande quantidade de inscrições")]
        public async Task Executar_Deve_Lidar_Com_Grande_Quantidade_De_Inscricoes()
        {
            // Arrange
            const int quantidadeInscricoes = 1000;
            var inscricoes = Enumerable.Range(1, quantidadeInscricoes)
                .Select(i => CriarInscricao(i, null!, null, new Usuario { Login = $"user{i}" }))
                .ToList();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(quantidadeInscricoes));
        }

        #endregion

        #region Testes de Rota e Comando

        [Fact(DisplayName = "Executar - Deve usar rota correta RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursistaTratar")]
        public async Task Executar_Deve_Usar_Rota_Correta()
        {
            // Arrange
            var inscricoes = new List<Inscricao>
            {
                CriarInscricao(1, null!, null, new Usuario { Login = "user1" })
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterInscricoesConfirmadasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inscricoes);

            var rotasCapturadas = new List<string>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<PublicarNaFilaRabbitCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) =>
                    {
                        var cmd = command as PublicarNaFilaRabbitCommand;
                        rotasCapturadas.Add(cmd!.Rota);
                    })
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(new MensagemRabbit());

            // Assert
            Assert.Single(rotasCapturadas);
            Assert.Equal(RotasRabbit.AtualizarCargoFuncaoVinculoInscricaoCursistaTratar, rotasCapturadas[0]);
        }

        #endregion

        #region Métodos Auxiliares

        private static Inscricao CriarInscricao(long id, string cargoCodigo, int? tipoVinculo, Usuario usuario)
        {
            return new Inscricao
            {
                Id = id,
                CargoCodigo = cargoCodigo,
                TipoVinculo = tipoVinculo,
                Usuario = usuario
            };
        }

        #endregion
    }
}
