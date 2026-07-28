using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ues;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterUnidadePorCodigoEolTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterUnidadePorCodigoEol _casoDeUso;

        public CasoDeUsoObterUnidadePorCodigoEolTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterUnidadePorCodigoEol(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar UnidadeEol com sucesso quando dados são válidos")]
        public async Task Executar_Deve_Retornar_UnidadeEol_Com_Sucesso_Quando_Dados_Validos()
        {
            // Arrange
            const string codigoEol = "123456";
            var unidadeEol = new UnidadeEol
            {
                Codigo = codigoEol,
                Sigla = "UE",
                NomeUnidade = "Unidade de Ensino Teste",
                Tipo = UnidadeEolTipo.Escola,
                CodigoReferencia = "REF123"
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(codigoEol, resultado.Codigo);
            Assert.Equal("UE", resultado.Sigla);
            Assert.Equal("Unidade de Ensino Teste", resultado.NomeUnidade);
            Assert.Equal(UnidadeEolTipo.Escola, resultado.Tipo);
            Assert.Equal("REF123", resultado.CodigoReferencia);
        }

        [Fact(DisplayName = "Executar - Deve criar ObterUnidadePorCodigoEOLQuery com código correto")]
        public async Task Executar_Deve_Criar_ObterUnidadePorCodigoEOLQuery_Com_Codigo_Correto()
        {
            // Arrange
            const string codigoEol = "789012";
            var unidadeEol = new UnidadeEol
            {
                Codigo = codigoEol,
                Sigla = "UE2",
                NomeUnidade = "Unidade 2"
            };

            ObterUnidadePorCodigoEOLQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UnidadeEol>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterUnidadePorCodigoEOLQuery)
                .ReturnsAsync(unidadeEol);

            // Act
            await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(codigoEol, queryCapturada.UnidadeCodigo);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            const string codigoEol = "345678";
            var unidadeEol = new UnidadeEol { Codigo = codigoEol };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            await _casoDeUso.Executar(codigoEol);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken_Para_Mediator()
        {
            // Arrange
            const string codigoEol = "567890";
            var unidadeEol = new UnidadeEol { Codigo = codigoEol };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            await _casoDeUso.Executar(codigoEol);

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono e retornar Task")]
        public async Task Executar_Deve_Ser_Assincrono_E_Retornar_Task()
        {
            // Arrange
            const string codigoEol = "111111";
            var unidadeEol = new UnidadeEol { Codigo = codigoEol };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            var tarefa = _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<UnidadeEol>>(tarefa);

            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar UnidadeEol")]
        public async Task Executar_Deve_Retornar_UnidadeEol()
        {
            // Arrange
            const string codigoEol = "222222";
            var unidadeEol = new UnidadeEol { Codigo = codigoEol };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.IsType<UnidadeEol>(resultado);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterUnidadePorCodigoEol)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterUnidadePorCodigoEol deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterUnidadePorCodigoEol")]
        public void Executar_Deve_Implementar_Interface_ICasoDeUsoObterUnidadePorCodigoEol()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterUnidadePorCodigoEol)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterUnidadePorCodigoEol"),
                "CasoDeUsoObterUnidadePorCodigoEol deve implementar ICasoDeUsoObterUnidadePorCodigoEol");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Com_IMediator()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterUnidadePorCodigoEol(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoObterUnidadePorCodigoEol>(casoDeUso);
            Assert.IsType<ICasoDeUsoObterUnidadePorCodigoEol>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterUnidadePorCodigoEol(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var campoMediator = typeof(CasoDeUsoObterUnidadePorCodigoEol)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve processar codigo com caracteres especiais")]
        public async Task Executar_Deve_Processar_Codigo_Com_Caracteres_Especiais()
        {
            // Arrange
            const string codigoEol = "ABC-123-DEF";
            var unidadeEol = new UnidadeEol { Codigo = codigoEol };

            ObterUnidadePorCodigoEOLQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UnidadeEol>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterUnidadePorCodigoEOLQuery)
                .ReturnsAsync(unidadeEol);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(codigoEol, queryCapturada.UnidadeCodigo);
            Assert.Equal(codigoEol, resultado.Codigo);
        }

        [Fact(DisplayName = "Executar - Deve preservar todas as propriedades da UnidadeEol retornada")]
        public async Task Executar_Deve_Preservar_Todas_Propriedades_Da_UnidadeEol()
        {
            // Arrange
            const string codigoEol = "999888";
            var unidadeEol = new UnidadeEol
            {
                Codigo = "999888",
                Sigla = "SIGLA",
                NomeUnidade = "NomeSocial da Unidade",
                Tipo = UnidadeEolTipo.Instituicao,
                CodigoReferencia = "REFXYZ"
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(unidadeEol.Codigo, resultado.Codigo);
            Assert.Equal(unidadeEol.Sigla, resultado.Sigla);
            Assert.Equal(unidadeEol.NomeUnidade, resultado.NomeUnidade);
            Assert.Equal(unidadeEol.Tipo, resultado.Tipo);
            Assert.Equal(unidadeEol.CodigoReferencia, resultado.CodigoReferencia);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado do mediator sem modificações")]
        public async Task Executar_Deve_Retornar_Resultado_Do_Mediator_Sem_Modificacoes()
        {
            // Arrange
            const string codigoEol = "333444";
            var unidadeEolOriginal = new UnidadeEol
            {
                Codigo = codigoEol,
                Sigla = "ORIGINAL",
                NomeUnidade = "Original Unit"
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEolOriginal);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.Same(unidadeEolOriginal, resultado);
        }

        [Fact(DisplayName = "Executar - Deve passar codigo vazio para query")]
        public async Task Executar_Deve_Passar_Codigo_Vazio_Para_Query()
        {
            // Arrange
            const string codigoEol = "";
            var unidadeEol = new UnidadeEol();

            ObterUnidadePorCodigoEOLQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UnidadeEol>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterUnidadePorCodigoEOLQuery)
                .ReturnsAsync(unidadeEol);

            // Act
            await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Equal(codigoEol, queryCapturada.UnidadeCodigo);
        }

        [Fact(DisplayName = "Executar - Deve manter referência de identidade com UnidadeEol retornada")]
        public async Task Executar_Deve_Manter_Referencia_De_Identidade_Com_UnidadeEol()
        {
            // Arrange
            const string codigoEol = "555666";
            var unidadeEolOriginal = new UnidadeEol 
            { 
                Codigo = codigoEol,
                NomeUnidade = "Test Unit"
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEolOriginal);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.Same(unidadeEolOriginal, resultado);
            Assert.True(object.ReferenceEquals(unidadeEolOriginal, resultado));
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado mesmo quando propriedades são null")]
        public async Task Executar_Deve_Retornar_Resultado_Mesmo_Quando_Propriedades_Null()
        {
            // Arrange
            const string codigoEol = "777888";
            var unidadeEol = new UnidadeEol
            {
                Codigo = codigoEol,
                Sigla = null!,
                NomeUnidade = null!,
                CodigoReferencia = null!
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(unidadeEol);

            // Act
            var resultado = await _casoDeUso.Executar(codigoEol);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(codigoEol, resultado.Codigo);
            Assert.Null(resultado.Sigla);
            Assert.Null(resultado.NomeUnidade);
            Assert.Null(resultado.CodigoReferencia);
        }

        [Fact(DisplayName = "Executar - Deve passar codigo null para query")]
        public async Task Executar_Deve_Passar_Codigo_Null_Para_Query()
        {
            // Arrange
            var unidadeEol = new UnidadeEol();

            ObterUnidadePorCodigoEOLQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<UnidadeEol>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterUnidadePorCodigoEOLQuery)
                .ReturnsAsync(unidadeEol);

            // Act
            await _casoDeUso.Executar(null!);

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Null(queryCapturada.UnidadeCodigo);
        }

        [Fact(DisplayName = "Executar - Deve ser chamado múltiplas vezes sem problema")]
        public async Task Executar_Deve_Ser_Chamado_Multiplas_Vezes_Sem_Problema()
        {
            // Arrange
            const string codigoEol1 = "111111";
            const string codigoEol2 = "222222";
            var unidadeEol1 = new UnidadeEol { Codigo = codigoEol1 };
            var unidadeEol2 = new UnidadeEol { Codigo = codigoEol2 };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ObterUnidadePorCodigoEOLQuery q, CancellationToken ct) => 
                    q.UnidadeCodigo == codigoEol1 ? unidadeEol1 : unidadeEol2);

            // Act
            var resultado1 = await _casoDeUso.Executar(codigoEol1);
            var resultado2 = await _casoDeUso.Executar(codigoEol2);

            // Assert
            Assert.Equal(codigoEol1, resultado1.Codigo);
            Assert.Equal(codigoEol2, resultado2.Codigo);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUnidadePorCodigoEOLQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }
    }
}
