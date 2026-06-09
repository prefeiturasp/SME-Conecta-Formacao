using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratarTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar _casoDeUso;

        public CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratarTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar(_mediatorMock.Object);
        }

        #region Testes de Comportamento Positivo

        [Fact(DisplayName = "Executar - Deve retornar true quando há cargos/funções disponíveis")]
        public async Task Executar_Deve_Retornar_True_Quando_Ha_Cargos_Funcoes()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_BASE", 101, "DRE001", "UE001")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar false quando não há cargos/funções disponíveis")]
        public async Task Executar_Deve_Retornar_False_Quando_Sem_Cargos_Funcoes()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CursistaCargoServicoEol>());

            // Act
            var resultado = await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.False(resultado);
        }

        [Fact(DisplayName = "Executar - Deve processar com sucesso quando DTO é válido")]
        public async Task Executar_Deve_Processar_Com_Sucesso_Quando_Dto_Valido()
        {
            // Arrange
            const long inscricaoId = 456;
            const string login = "rf.teste";
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = inscricaoId,
                Login = login,
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(2001, "CARGO_A", 201, "DRE002", "UE002")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.True(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region Testes de Exceção

        [Fact(DisplayName = "Executar - Deve lançar NegocioException quando DTO é nulo")]
        public async Task Executar_Deve_Lancar_Excecao_Quando_Dto_Nulo()
        {
            // Arrange
            var mensagem = new MensagemRabbit(string.Empty);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagem));

            Assert.Equal(MensagemNegocio.ATUALIZACAO_VINCULO_INSCRICAO_NAO_LOCALIZADA, excecao.Message);
        }

        [Fact(DisplayName = "Executar - Deve lançar NegocioException com mensagem correta")]
        public async Task Executar_Deve_Lancar_Excecao_Com_Mensagem_Correta()
        {
            // Arrange
            var mensagem = new MensagemRabbit("invalid json");

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => _casoDeUso.Executar(mensagem));

            Assert.Equal(MensagemNegocio.ATUALIZACAO_VINCULO_INSCRICAO_NAO_LOCALIZADA, excecao.Message);
        }

        #endregion

        #region Testes de Filtragem de Cargos

        [Fact(DisplayName = "Executar - Deve filtrar cargos quando CargoCodigo é informado")]
        public async Task Executar_Deve_Filtrar_Cargos_Quando_CargoCodigo_Informado()
        {
            // Arrange
            const string cargoCodigoFiltro = "1001";
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = cargoCodigoFiltro
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001"),
                CriarCursistaCargoServicoEol(1002, "CARGO_B", 102, "DRE002", "UE002")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.ToList();
            Assert.All(dadosInscricao, d => Assert.Equal(cargoCodigoFiltro, d.Codigo));
        }

        [Fact(DisplayName = "Executar - Não deve filtrar cargos quando CargoCodigo é nulo")]
        public async Task Executar_Nao_Deve_Filtrar_Cargos_Quando_CargoCodigo_Nulo()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001"),
                CriarCursistaCargoServicoEol(1002, "CARGO_B", 102, "DRE002", "UE002")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.ToList();
            Assert.True(dadosInscricao.Count >= 2);
        }

        #endregion

        #region Testes de Mapeamento de Dados - Cargo Base

        [Fact(DisplayName = "Executar - Deve mapear cargo base corretamente")]
        public async Task Executar_Deve_Mapear_Cargo_Base_Corretamente()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            const long cdCargoBase = 3001;
            const string cargoBaseDescricao = "PROFESSOR";
            const string dreCargoBase = "DRE_001";
            const string ueCargoBase = "UE_001";
            const int tipoVinculoCargoBase = 5;
            var dataInicioCargoBase = new DateTime(2023, 01, 15);

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = cdCargoBase,
                    CargoBase = cargoBaseDescricao,
                    CdDreCargoBase = dreCargoBase,
                    CdUeCargoBase = ueCargoBase,
                    TipoVinculoCargoBase = tipoVinculoCargoBase,
                    DataInicioCargoBase = dataInicioCargoBase
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.First();
            Assert.Equal(cdCargoBase.ToString(), dadosInscricao.Codigo);
            Assert.Equal(cargoBaseDescricao, dadosInscricao.Descricao);
            Assert.Equal(dreCargoBase, dadosInscricao.DreCodigo);
            Assert.Equal(ueCargoBase, dadosInscricao.UeCodigo);
            Assert.Equal(tipoVinculoCargoBase, dadosInscricao.TipoVinculo);
            Assert.Equal(dataInicioCargoBase, dadosInscricao.DataInicio);
        }

        #endregion

        #region Testes de Mapeamento de Dados - Função Atividade

        [Fact(DisplayName = "Executar - Deve mapear função atividade quando presente")]
        public async Task Executar_Deve_Mapear_Funcao_Atividade_Quando_Presente()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            const long cdFuncaoAtividade = 4001;
            const string funcaoAtividadeDescricao = "FUNCAO_ESPECIAL";
            const string dreFuncaoAtividade = "DRE_002";
            const string ueFuncaoAtividade = "UE_002";
            const int tipoVinculoFuncaoAtividade = 3;
            var dataInicioFuncaoAtividade = new DateTime(2023, 06, 20);

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "CARGO_BASE",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = 1,
                    DataInicioCargoBase = null,
                    CdFuncaoAtividade = cdFuncaoAtividade,
                    FuncaoAtividade = funcaoAtividadeDescricao,
                    CdDreFuncaoAtividade = dreFuncaoAtividade,
                    CdUeFuncaoAtividade = ueFuncaoAtividade,
                    TipoVinculoFuncaoAtividade = tipoVinculoFuncaoAtividade,
                    DataInicioFuncaoAtividade = dataInicioFuncaoAtividade
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var cargoBase = commandCapturado.DadosInscricao.First();
            Assert.NotEmpty(cargoBase.Funcoes);
            var funcao = cargoBase.Funcoes.First();
            Assert.Equal(cdFuncaoAtividade.ToString(), funcao.Codigo);
            Assert.Equal(funcaoAtividadeDescricao, funcao.Descricao);
            Assert.Equal(dreFuncaoAtividade, funcao.DreCodigo);
            Assert.Equal(ueFuncaoAtividade, funcao.UeCodigo);
            Assert.Equal(tipoVinculoFuncaoAtividade, funcao.TipoVinculo);
            Assert.Equal(dataInicioFuncaoAtividade, funcao.DataInicio);
        }

        [Fact(DisplayName = "Executar - Não deve adicionar função quando CdFuncaoAtividade é nulo")]
        public async Task Executar_Nao_Deve_Adicionar_Funcao_Quando_Nula()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "CARGO_BASE",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = 1,
                    CdFuncaoAtividade = null,
                    FuncaoAtividade = null!
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var cargoBase = commandCapturado.DadosInscricao.First();
            Assert.Empty(cargoBase.Funcoes);
        }

        #endregion

        #region Testes de Mapeamento de Dados - Cargo Sobreposto

        [Fact(DisplayName = "Executar - Deve mapear cargo sobreposto quando presente")]
        public async Task Executar_Deve_Mapear_Cargo_Sobreposto_Quando_Presente()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            const long cdCargoSobreposto = 5001;
            const string cargoSobrepostoDescricao = "CARGO_SOBREPOSTO";
            const string dreCargoSobreposto = "DRE_003";
            const string ueCargoSobreposto = "UE_003";
            const int tipoVinculoCargoSobreposto = 2;
            var dataInicioCargoSobreposto = new DateTime(2024, 03, 10);

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "CARGO_BASE",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = 1,
                    CdCargoSobreposto = cdCargoSobreposto,
                    CargoSobreposto = cargoSobrepostoDescricao,
                    CdDreCargoSobreposto = dreCargoSobreposto,
                    CdUeCargoSobreposto = ueCargoSobreposto,
                    TipoVinculoCargoSobreposto = tipoVinculoCargoSobreposto,
                    DataInicioCargoSobreposto = dataInicioCargoSobreposto
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.ToList();
            Assert.Equal(2, dadosInscricao.Count);
            
            var cargoSobreposto = dadosInscricao[1];
            Assert.Equal(cdCargoSobreposto.ToString(), cargoSobreposto.Codigo);
            Assert.Equal(cargoSobrepostoDescricao, cargoSobreposto.Descricao);
            Assert.Equal(dreCargoSobreposto, cargoSobreposto.DreCodigo);
            Assert.Equal(ueCargoSobreposto, cargoSobreposto.UeCodigo);
            Assert.Equal(tipoVinculoCargoSobreposto, cargoSobreposto.TipoVinculo);
            Assert.Equal(dataInicioCargoSobreposto, cargoSobreposto.DataInicio);
        }

        [Fact(DisplayName = "Executar - Não deve adicionar cargo sobreposto quando CdCargoSobreposto é nulo")]
        public async Task Executar_Nao_Deve_Adicionar_Cargo_Sobreposto_Quando_Nulo()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "CARGO_BASE",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = 1,
                    CdCargoSobreposto = null,
                    CargoSobreposto = null
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.ToList();
            Assert.Single(dadosInscricao);
        }

        #endregion

        #region Testes de Casos Complexos

        [Fact(DisplayName = "Executar - Deve processar múltiplos cargos com funções e sobreposições")]
        public async Task Executar_Deve_Processar_Multiplos_Cargos_Com_Funcoes_E_Sobreposicoes()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "PROFESSOR",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = 1,
                    DataInicioCargoBase = new DateTime(2023, 01, 15),
                    CdFuncaoAtividade = 4001,
                    FuncaoAtividade = "GESTOR",
                    CdDreFuncaoAtividade = "DRE_001",
                    CdUeFuncaoAtividade = "UE_001",
                    TipoVinculoFuncaoAtividade = 2,
                    DataInicioFuncaoAtividade = new DateTime(2023, 06, 01),
                    CdCargoSobreposto = 5001,
                    CargoSobreposto = "EDUCADOR",
                    CdDreCargoSobreposto = "DRE_002",
                    CdUeCargoSobreposto = "UE_002",
                    TipoVinculoCargoSobreposto = 3,
                    DataInicioCargoSobreposto = new DateTime(2024, 01, 01)
                },
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3002,
                    CargoBase = "DIRETOR",
                    CdDreCargoBase = "DRE_003",
                    CdUeCargoBase = "UE_003",
                    TipoVinculoCargoBase = 4,
                    DataInicioCargoBase = new DateTime(2022, 05, 10)
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.True(resultado);
            Assert.NotNull(commandCapturado);
            var dadosInscricao = commandCapturado.DadosInscricao.ToList();
            
            // Verificar que há 3 itens: cargo base 1, cargo sobreposto 1, cargo base 2
            Assert.Equal(3, dadosInscricao.Count);
            
            // Primeiro cargo base
            Assert.Equal("3001", dadosInscricao[0].Codigo);
            Assert.Equal("PROFESSOR", dadosInscricao[0].Descricao);
            Assert.NotEmpty(dadosInscricao[0].Funcoes);
            Assert.Single(dadosInscricao[0].Funcoes);
            Assert.Equal("4001", dadosInscricao[0].Funcoes.First().Codigo);
            
            // Cargo sobreposto do primeiro cargo
            Assert.Equal("5001", dadosInscricao[1].Codigo);
            Assert.Equal("EDUCADOR", dadosInscricao[1].Descricao);
            
            // Segundo cargo base
            Assert.Equal("3002", dadosInscricao[2].Codigo);
            Assert.Equal("DIRETOR", dadosInscricao[2].Descricao);
            Assert.Empty(dadosInscricao[2].Funcoes);
        }

        [Fact(DisplayName = "Executar - Deve processar cargo com tipo vínculo nulo (padrão 0)")]
        public async Task Executar_Deve_Processar_Cargo_Com_Tipo_Vinculo_Nulo()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                new CursistaCargoServicoEol
                {
                    CdCargoBase = 3001,
                    CargoBase = "CARGO_BASE",
                    CdDreCargoBase = "DRE_001",
                    CdUeCargoBase = "UE_001",
                    TipoVinculoCargoBase = null
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            var cargoBase = commandCapturado.DadosInscricao.First();
            Assert.Equal(0, cargoBase.TipoVinculo);
        }

        #endregion

        #region Testes de Chamadas ao Mediator

        [Fact(DisplayName = "Executar - Deve chamar ObterCargosFuncoesDresFuncionarioServicoEolQuery com login correto")]
        public async Task Executar_Deve_Chamar_Query_Com_Login_Correto()
        {
            // Arrange
            const string loginEsperado = "rf.usuario.teste";
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = loginEsperado,
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(
                        q => q.RegistroFuncional == loginEsperado),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand com ID correto")]
        public async Task Executar_Deve_Chamar_Command_Com_Id_Correto()
        {
            // Arrange
            const long idEsperado = 999999;
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = idEsperado,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand? commandCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (command, ct) => commandCapturado = command as AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand)
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(commandCapturado);
            Assert.Equal(idEsperado, commandCapturado!.Id);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator exatamente 2 vezes quando há cargos")]
        public async Task Executar_Deve_Chamar_Mediator_Exatamente_Duas_Vezes()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator apenas 1 vez quando não há cargos")]
        public async Task Executar_Deve_Chamar_Mediator_Uma_Vez_Quando_Sem_Cargos()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CursistaCargoServicoEol>());

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        #endregion

        #region Testes de Assincronia e Thread-Safety

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CursistaCargoServicoEol>());

            // Act
            var tarefa = _casoDeUso.Executar(mensagem);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<bool>>(tarefa);
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para todas as chamadas")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            var dto = new AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto
            {
                Id = 123,
                Login = "rf.usuario",
                CargoCodigo = null
            };
            var mensagem = new MensagemRabbit(SerializarDto(dto));

            var cargosFuncoes = new List<CursistaCargoServicoEol>
            {
                CriarCursistaCargoServicoEol(1001, "CARGO_A", 101, "DRE001", "UE001")
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoes);

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _casoDeUso.Executar(mensagem);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<IRequest<object>>(),
                    It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }

        #endregion

        #region Testes de Estrutura e Interface

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar"),
                "CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar deve implementar ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act & Assert
            var casoDeUso = new CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar(_mediatorMock.Object);
            Assert.NotNull(casoDeUso);
        }

        #endregion

        #region Métodos Auxiliares

        private static string SerializarDto(AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto dto)
        {
            return JsonSerializer.Serialize(dto);
        }

        private static CursistaCargoServicoEol CriarCursistaCargoServicoEol(
            long cdCargoBase, 
            string cargoBase, 
            int? tipoVinculo, 
            string dreCodigo, 
            string ueCodigo)
        {
            return new CursistaCargoServicoEol
            {
                CdCargoBase = cdCargoBase,
                CargoBase = cargoBase,
                CdDreCargoBase = dreCodigo,
                CdUeCargoBase = ueCodigo,
                TipoVinculoCargoBase = tipoVinculo,
                DataInicioCargoBase = null
            };
        }

        #endregion
    }
}
