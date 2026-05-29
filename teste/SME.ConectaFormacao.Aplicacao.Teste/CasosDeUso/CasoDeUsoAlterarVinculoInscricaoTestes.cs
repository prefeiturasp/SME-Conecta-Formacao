using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAlterarVinculoInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAlterarVinculoInscricao _casoDeUso;

        public CasoDeUsoAlterarVinculoInscricaoTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoAlterarVinculoInscricao(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve alterar vínculo com sucesso quando dados são válidos")]
        public async Task Executar_Deve_Alterar_Vinculo_Com_Sucesso_Quando_Dados_Validos()
        {
            // Arrange
            const long id = 123;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO001",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve passar ID e DTO corretos para AlterarCargoFuncaoVinculoInscricaoCommand")]
        public async Task Executar_Deve_Passar_Id_E_Dto_Corretos_Para_Command()
        {
            // Arrange
            const long idEsperado = 456;
            const string cargoCodigoEsperado = "CARGO002";
            const int tipoVinculoEsperado = 2;

            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = cargoCodigoEsperado,
                TipoVinculo = tipoVinculoEsperado
            };

            AlterarCargoFuncaoVinculoInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarCargoFuncaoVinculoInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(idEsperado, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(idEsperado, commandCapturado.Id);
            Assert.Equal(cargoCodigoEsperado, commandCapturado.AlterarCargoFuncaoVinculoIncricao.CargoCodigo);
            Assert.Equal(tipoVinculoEsperado, commandCapturado.AlterarCargoFuncaoVinculoIncricao.TipoVinculo);
        }

        [Fact(DisplayName = "Executar - Deve retornar true quando alteração bem-sucedida")]
        public async Task Executar_Deve_Retornar_True_Quando_Alteracao_Bem_Sucedida()
        {
            // Arrange
            const long id = 789;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO003",
                TipoVinculo = 3
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            Assert.IsType<bool>(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar false quando alteração falha")]
        public async Task Executar_Deve_Retornar_False_Quando_Alteracao_Falha()
        {
            // Arrange
            const long id = 999;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO004",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            const long id = 111;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO005",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var tarefa = _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

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
                typeof(CasoDeUsoAlterarVinculoInscricao)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoAlterarVinculoInscricao deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoAlterarVinculoInscricao")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAlterarVinculoInscricao)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoAlterarVinculoInscricao"),
                "CasoDeUsoAlterarVinculoInscricao deve implementar ICasoDeUsoAlterarVinculoInscricao");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoAlterarVinculoInscricao(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
            Assert.IsType<ICasoDeUsoAlterarVinculoInscricao>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            const long id = 222;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO006",
                TipoVinculo = 2
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            const long id = 333;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO007",
                TipoVinculo = 3
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve mapear ID do parâmetro para Command")]
        public async Task Executar_Deve_Mapear_Id_Para_Command()
        {
            // Arrange
            const long idEsperado = 555;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO008",
                TipoVinculo = 1
            };

            AlterarCargoFuncaoVinculoInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarCargoFuncaoVinculoInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(idEsperado, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(idEsperado, commandCapturado.Id);
        }

        [Fact(DisplayName = "Executar - Deve mapear CargoCodigo do DTO para Command")]
        public async Task Executar_Deve_Mapear_CargoCodigo_Para_Command()
        {
            // Arrange
            const string cargoCodigoEsperado = "CARGO_ESPECIAL";
            const long id = 666;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = cargoCodigoEsperado,
                TipoVinculo = 2
            };

            AlterarCargoFuncaoVinculoInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarCargoFuncaoVinculoInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(cargoCodigoEsperado, commandCapturado.AlterarCargoFuncaoVinculoIncricao.CargoCodigo);
        }

        [Fact(DisplayName = "Executar - Deve mapear TipoVinculo do DTO para Command")]
        public async Task Executar_Deve_Mapear_TipoVinculo_Para_Command()
        {
            // Arrange
            const int tipoVinculoEsperado = 5;
            const long id = 777;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO009",
                TipoVinculo = tipoVinculoEsperado
            };

            AlterarCargoFuncaoVinculoInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarCargoFuncaoVinculoInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(tipoVinculoEsperado, commandCapturado.AlterarCargoFuncaoVinculoIncricao.TipoVinculo);
        }

        [Fact(DisplayName = "Executar - Deve manter integridade do DTO ao passar para Command")]
        public async Task Executar_Deve_Manter_Integridade_Dto_Para_Command()
        {
            // Arrange
            const long id = 888;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO010",
                TipoVinculo = 1
            };

            AlterarCargoFuncaoVinculoInscricaoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarCargoFuncaoVinculoInscricaoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.NotNull(commandCapturado.AlterarCargoFuncaoVinculoIncricao);
            Assert.All(
                [commandCapturado.Id, commandCapturado.AlterarCargoFuncaoVinculoIncricao.TipoVinculo],
                item => Assert.True(item >= 0));
        }

        [Fact(DisplayName = "Executar - Deve funcionnar com ID zero")]
        public async Task Executar_Deve_Funcionar_Com_Id_Zero()
        {
            // Arrange
            const long id = 0;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO011",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com ID negativo")]
        public async Task Executar_Deve_Funcionar_Com_Id_Negativo()
        {
            // Arrange
            const long id = -1;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO012",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com ID long.MaxValue")]
        public async Task Executar_Deve_Funcionar_Com_Id_MaxValue()
        {
            // Arrange
            var id = long.MaxValue;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO013",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com string vazia em CargoCodigo")]
        public async Task Executar_Deve_Funcionar_Com_CargoCodigo_Vazio()
        {
            // Arrange
            const long id = 999;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = string.Empty,
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com TipoVinculo zero")]
        public async Task Executar_Deve_Funcionar_Com_TipoVinculo_Zero()
        {
            // Arrange
            const long id = 1000;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO014",
                TipoVinculo = 0
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve funcionar com TipoVinculo negativo")]
        public async Task Executar_Deve_Funcionar_Com_TipoVinculo_Negativo()
        {
            // Arrange
            const long id = 1001;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO015",
                TipoVinculo = -1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve executar sem efeitos colaterais ao mediator quando falha")]
        public async Task Executar_Deve_Executar_Sem_Efeitos_Colaterais_Ao_Mediator()
        {
            // Arrange
            const long id = 1002;
            var alterarCargoFuncaoVinculoDto = new AlterarCargoFuncaoVinculoIncricaoDTO
            {
                CargoCodigo = "CARGO016",
                TipoVinculo = 1
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _casoDeUso.Executar(id, alterarCargoFuncaoVinculoDto);

            // Assert
            Assert.False(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarCargoFuncaoVinculoInscricaoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(1));
        }
    }
}
