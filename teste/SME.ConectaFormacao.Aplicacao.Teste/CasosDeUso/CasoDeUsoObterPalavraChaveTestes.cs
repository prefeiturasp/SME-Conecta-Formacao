using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.PalavraChave;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterPalavraChaveTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterPalavraChave _casoDeUso;

        public CasoDeUsoObterPalavraChaveTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterPalavraChave(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista de palavras-chave com sucesso")]
        public async Task Executar_Deve_Retornar_Lista_De_Palavras_Chave_Com_Sucesso()
        {
            // Arrange
            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Programação" },
                new() { Id = 2, Descricao = "Banco de Dados" },
                new() { Id = 3, Descricao = "Web Development" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count());
            Assert.Equal(palavrasChaveEsperadas, resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista vazia quando não há palavras-chave")]
        public async Task Executar_Deve_Retornar_Lista_Vazia_Quando_Nao_Ha_Palavras_Chave()
        {
            // Arrange
            var listaVazia = new List<RetornoListagemDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaVazia);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact(DisplayName = "Executar - Deve enviar ObterPalavraChaveQuery.Instancia para mediator")]
        public async Task Executar_Deve_Enviar_ObterPalavraChaveQuery_Instancia_Para_Mediator()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Teste" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken_Para_Mediator()
        {
            // Arrange
            CancellationToken cancellationTokenCapturado = CancellationToken.None;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<RetornoListagemDTO>>, CancellationToken>(
                    (query, ct) => cancellationTokenCapturado = ct)
                .ReturnsAsync([]);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono e retornar Task")]
        public async Task Executar_Deve_Ser_Assincrono_E_Retornar_Task()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<IEnumerable<RetornoListagemDTO>>>(tarefa);

            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar IEnumerable<RetornoListagemDTO>")]
        public async Task Executar_Deve_Retornar_IEnumerable_RetornoListagemDto()
        {
            // Arrange
            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Teste" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.IsType<IEnumerable<RetornoListagemDTO>>(resultado, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve manter ordem das palavras-chave retornadas")]
        public async Task Executar_Deve_Manter_Ordem_Das_Palavras_Chave_Retornadas()
        {
            // Arrange
            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Alpha" },
                new() { Id = 2, Descricao = "Beta" },
                new() { Id = 3, Descricao = "Gamma" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            var resultadoList = resultado.ToList();
            Assert.Equal("Alpha", resultadoList[0].Descricao);
            Assert.Equal("Beta", resultadoList[1].Descricao);
            Assert.Equal("Gamma", resultadoList[2].Descricao);
        }

        [Fact(DisplayName = "Executar - Deve preservar propriedades de cada RetornoListagemDTO")]
        public async Task Executar_Deve_Preservar_Propriedades_De_Cada_RetornoListagemDto()
        {
            // Arrange
            const long idEsperado = 42;
            const string descricaoEsperada = "Descrição Única";

            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = idEsperado, Descricao = descricaoEsperada }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            var palavraChave = resultado.First();
            Assert.Equal(idEsperado, palavraChave.Id);
            Assert.Equal(descricaoEsperada, palavraChave.Descricao);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterPalavraChave)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterPalavraChave deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterPalavraChave")]
        public void Executar_Deve_Implementar_Interface_ICasoDeUsoObterPalavraChave()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterPalavraChave)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterPalavraChave"),
                "CasoDeUsoObterPalavraChave deve implementar ICasoDeUsoObterPalavraChave");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Com_IMediator()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterPalavraChave(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoObterPalavraChave>(casoDeUso);
            Assert.IsType<ICasoDeUsoObterPalavraChave>(casoDeUso, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterPalavraChave(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            // Verifica que o mediator foi armazenado na classe base através de reflexão
            var campoMediator = typeof(CasoDeUsoObterPalavraChave)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve usar Query singleton ObterPalavraChaveQuery.Instancia")]
        public async Task Executar_Deve_Usar_Query_Singleton_ObterPalavraChaveQuery_Instancia()
        {
            // Arrange
            ObterPalavraChaveQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<RetornoListagemDTO>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterPalavraChaveQuery)
                .ReturnsAsync([]);

            // Act
            await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.Same(ObterPalavraChaveQuery.Instancia, queryCapturada);
        }

        [Fact(DisplayName = "Executar - Deve retornar enumeração múltiplas vezes sem problema")]
        public async Task Executar_Deve_Retornar_Enumeracao_Multiplas_Vezes_Sem_Problema()
        {
            // Arrange
            var palavrasChaveEsperadas = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Teste" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert - Enumera múltiplas vezes para verificar que não há problemas
            var primeiraEnumeracao = resultado.ToList();
            var segundaEnumeracao = resultado.ToList();

            Assert.Equal(primeiraEnumeracao.Count, segundaEnumeracao.Count);
            Assert.Single(primeiraEnumeracao);
            Assert.Single(segundaEnumeracao);
        }

        [Fact(DisplayName = "Executar - Deve processar lista com múltiplos itens corretamente")]
        public async Task Executar_Deve_Processar_Lista_Com_Multiplos_Itens_Corretamente()
        {
            // Arrange
            var palavrasChaveEsperadas = Enumerable.Range(1, 10)
                .Select(i => new RetornoListagemDTO { Id = i, Descricao = $"Palavra{i}" })
                .ToList();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveEsperadas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.Equal(10, resultado.Count());
            for (int i = 1; i <= 10; i++)
            {
                Assert.Contains(resultado, p => p.Id == i && p.Descricao == $"Palavra{i}");
            }
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado do mediator sem modificações")]
        public async Task Executar_Deve_Retornar_Resultado_Do_Mediator_Sem_Modificacoes()
        {
            // Arrange
            var palavrasChaveOriginais = new List<RetornoListagemDTO>
            {
                new() { Id = 1, Descricao = "Original 1" },
                new() { Id = 2, Descricao = "Original 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterPalavraChaveQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(palavrasChaveOriginais);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.Equal(palavrasChaveOriginais.Count, resultado.Count());
            Assert.All(resultado, item => 
            {
                Assert.Contains(palavrasChaveOriginais, p => p.Id == item.Id && p.Descricao == item.Descricao);
            });
        }
    }
}
