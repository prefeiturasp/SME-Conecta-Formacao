using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAreaPromotoraListaAreaPublicaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterAreaPromotoraListaAreaPublica _casoDeUso;

        public CasoDeUsoObterAreaPromotoraListaAreaPublicaTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterAreaPromotoraListaAreaPublica(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista de áreas públicas com sucesso")]
        public async Task Executar_Deve_Retornar_Lista_Areas_Publicas_Com_Sucesso()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área Pública 1" },
                new() { Id = 2, Descricao = "Área Pública 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(areasEsperadas.Count, resultado.Count());
            Assert.Equal(areasEsperadas, resultado);
        }

        [Fact(DisplayName = "Executar - Deve enviar query com parâmetro null para mediator")]
        public async Task Executar_Deve_Enviar_Query_Com_Parametro_Null()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterAreaPromotoraListaQuery>(q => q != null && q.AreaPromotoraIdUsuarioLogado == null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Exatamente_Uma_Vez()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<IEnumerable<RetornoListagemDTO>>>(tarefa);
            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterAreaPromotoraListaAreaPublica)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterAreaPromotoraListaAreaPublica deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterAreaPromotoraListaAreaPublica")]
        public void Executar_Deve_Implementar_Interface()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterAreaPromotoraListaAreaPublica)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterAreaPromotoraListaAreaPublica"),
                "CasoDeUsoObterAreaPromotoraListaAreaPublica deve implementar ICasoDeUsoObterAreaPromotoraListaAreaPublica");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor()
        {
            // Act
            var casoDeUso = new CasoDeUsoObterAreaPromotoraListaAreaPublica(_mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoObterAreaPromotoraListaAreaPublica>(casoDeUso);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado vazio quando não há áreas públicas")]
        public async Task Executar_Deve_Retornar_Resultado_Vazio_Quando_Nao_Ha_Areas()
        {
            // Arrange
            var areasEsperadas = Enumerable.Empty<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com múltiplas áreas")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Multiplas_Areas()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área 1" },
                new() { Id = 2, Descricao = "Área 2" },
                new() { Id = 3, Descricao = "Área 3" },
                new() { Id = 4, Descricao = "Área 4" },
                new() { Id = 5, Descricao = "Área 5" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(5, resultado.Count());
            var resultadoList = resultado.ToList();
            for (int i = 0; i < areasEsperadas.Count; i++)
            {
                Assert.Equal(areasEsperadas[i].Id, resultadoList[i].Id);
                Assert.Equal(areasEsperadas[i].Descricao, resultadoList[i].Descricao);
            }
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve retornar IEnumerable<RetornoListagemDTO>")]
        public async Task Executar_Deve_Retornar_IEnumerable_RetornoListagemDTO()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Teste" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.IsType<List<RetornoListagemDTO>>(resultado, exactMatch: false);
            Assert.IsType<IEnumerable<RetornoListagemDTO>>(resultado, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve preservar propriedades de RetornoListagemDTO")]
        public async Task Executar_Deve_Preservar_Propriedades_RetornoListagemDTO()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 42, Descricao = "Área Especial" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.Single(resultadoList);
            Assert.Equal(42, resultadoList[0].Id);
            Assert.Equal("Área Especial", resultadoList[0].Descricao);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterAreaPromotoraListaAreaPublica(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var campoMediator = typeof(CasoDeUsoObterAreaPromotoraListaAreaPublica)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
            Assert.Same(mediatorMock.Object, valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado consistente em múltiplas chamadas")]
        public async Task Executar_Deve_Retornar_Resultado_Consistente_Em_Multiplas_Chamadas()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área 1" },
                new() { Id = 2, Descricao = "Área 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado1 = await _casoDeUso.Executar();
            var resultado2 = await _casoDeUso.Executar();

            // Assert
            Assert.Equal(resultado1.Count(), resultado2.Count());
            var lista1 = resultado1.ToList();
            var lista2 = resultado2.ToList();

            for (int i = 0; i < lista1.Count; i++)
            {
                Assert.Equal(lista1[i].Id, lista2[i].Id);
                Assert.Equal(lista1[i].Descricao, lista2[i].Descricao);
            }

            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact(DisplayName = "Executar - Deve enumerar resultado múltiplas vezes sem problema")]
        public async Task Executar_Deve_Enumerar_Resultado_Multiplas_Vezes()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Área 1" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert - Enumera múltiplas vezes
            var primeiraEnumeracao = resultado.ToList();
            var segundaEnumeracao = resultado.ToList();
            var terceiraEnumeracao = resultado.Count();

            Assert.Single(primeiraEnumeracao);
            Assert.Single(segundaEnumeracao);
            Assert.Equal(1, terceiraEnumeracao);
        }

        [Fact(DisplayName = "Executar - Deve criar query com construtor correto")]
        public async Task Executar_Deve_Criar_Query_Com_Construtor_Correto()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();
            ObterAreaPromotoraListaQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<RetornoListagemDTO>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterAreaPromotoraListaQuery)
                .ReturnsAsync(areasEsperadas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Null(queryCapturada.AreaPromotoraIdUsuarioLogado);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista não-nula")]
        public async Task Executar_Deve_Retornar_Lista_Nao_Nula()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado do tipo Task")]
        public void Executar_Deve_Retornar_Tipo_Task()
        {
            // Arrange
            var areasEsperadas = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterAreaPromotoraListaQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areasEsperadas);

            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            Assert.NotNull(tarefa);
            Assert.IsType<Task>(tarefa, exactMatch: false);
        }
    }
}
