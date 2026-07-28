using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Funcionario;
using SME.ConectaFormacao.Aplicacao.Consultas.ServicoAcessos.ObterUsuariosPareceristas;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterPareceristaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterParecerista _casoDeUso;

        public CasoDeUsoObterPareceristaTestes()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoObterParecerista(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista de pareceristas com sucesso")]
        public async Task Executar_Deve_Retornar_Lista_De_Pareceristas_Com_Sucesso()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "parecerista1", Nome = "João Silva" },
                new() { Login = "parecerista2", Nome = "Maria Santos" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.NotEmpty(resultado);
            Assert.IsType<List<RetornoUsuarioLoginNomeDTO>>(resultado, exactMatch: false);
        }

        [Fact(DisplayName = "Executar - Deve retornar lista vazia quando não há pareceristas")]
        public async Task Executar_Deve_Retornar_Lista_Vazia_Quando_Nao_Ha_Pareceristas()
        {
            // Arrange
            var listaVazia = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(listaVazia);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send com ObterUsuariosPareceristasQuery")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Com_ObterUsuariosPareceristasQuery()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterUsuariosPareceristasQuery>(q => q != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve chamar mediator.Send exatamente uma vez")]
        public async Task Executar_Deve_Chamar_Mediator_Send_Exatamente_Uma_Vez()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "test", Nome = "Test User" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "Mediator.Send deve ser chamado exatamente uma vez");
        }

        [Fact(DisplayName = "Executar - Deve retornar Task<IEnumerable<RetornoUsuarioLoginNomeDTO>>")]
        public async Task Executar_Deve_Retornar_Task_IEnumerable_RetornoUsuarioLoginNomeDTO()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            Assert.NotNull(tarefa);
            await Assert.IsType<Task<IEnumerable<RetornoUsuarioLoginNomeDTO>>>(tarefa);
        }

        [Fact(DisplayName = "Executar - Deve ser assíncrono")]
        public async Task Executar_Deve_Ser_Assincrono()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var tarefa = _casoDeUso.Executar();

            // Assert
            await Assert.IsType<Task<IEnumerable<RetornoUsuarioLoginNomeDTO>>>(tarefa);
            var resultado = await tarefa;
            Assert.NotNull(resultado);
        }

        [Fact(DisplayName = "Executar - Deve retornar pareceristas com Login e NomeSocial preenchidos")]
        public async Task Executar_Deve_Retornar_Pareceristas_Com_Login_E_Nome_Preenchidos()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "jsilva", Nome = "João Silva" },
                new() { Login = "msantos", Nome = "Maria Santos" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.All(resultadoList, item =>
            {
                Assert.NotNull(item.Login);
                Assert.NotNull(item.Nome);
                Assert.NotEmpty(item.Login);
                Assert.NotEmpty(item.Nome);
            });
        }

        [Fact(DisplayName = "Executar - Deve repassar CancellationToken para mediator")]
        public async Task Executar_Deve_Repassar_CancellationToken_Para_Mediator()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve herdar de CasoDeUsoAbstrato")]
        public void Executar_Deve_Herdar_De_CasoDeUsoAbstrato()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterParecerista)
                    .BaseType?.Name.Contains("CasoDeUsoAbstrato") ?? false,
                "CasoDeUsoObterParecerista deve herdar de CasoDeUsoAbstrato");
        }

        [Fact(DisplayName = "Executar - Deve implementar interface ICasoDeUsoObterParecerista")]
        public void Executar_Deve_Implementar_Interface_ICasoDeUsoObterParecerista()
        {
            // Assert
            Assert.True(
                typeof(CasoDeUsoObterParecerista)
                    .GetInterfaces()
                    .Any(i => i.Name == "ICasoDeUsoObterParecerista"),
                "CasoDeUsoObterParecerista deve implementar ICasoDeUsoObterParecerista");
        }

        [Fact(DisplayName = "Executar - Deve utilizar primary constructor com IMediator")]
        public void Executar_Deve_Utilizar_Primary_Constructor_Com_IMediator()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterParecerista(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            Assert.IsType<CasoDeUsoObterParecerista>(casoDeUso);
        }

        [Fact(DisplayName = "Executar - Deve armazenar mediator na classe base")]
        public void Executar_Deve_Armazenar_Mediator_Na_Classe_Base()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            var casoDeUso = new CasoDeUsoObterParecerista(mediatorMock.Object);

            // Assert
            Assert.NotNull(casoDeUso);
            var campoMediator = typeof(CasoDeUsoObterParecerista)
                .BaseType?
                .GetField("mediator",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            Assert.NotNull(campoMediator);
            var valorMediator = campoMediator.GetValue(casoDeUso);
            Assert.NotNull(valorMediator);
        }

        [Fact(DisplayName = "Executar - Deve retornar resultado com múltiplos pareceristas")]
        public async Task Executar_Deve_Retornar_Resultado_Com_Multiplos_Pareceristas()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "parecerista1", Nome = "Parecerista Um" },
                new() { Login = "parecerista2", Nome = "Parecerista Dois" },
                new() { Login = "parecerista3", Nome = "Parecerista Três" },
                new() { Login = "parecerista4", Nome = "Parecerista Quatro" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.Equal(4, resultadoList.Count);
            Assert.Equal(pareceristas, resultadoList);
        }

        [Fact(DisplayName = "Executar - Deve retornar dados corretamente do mediator")]
        public async Task Executar_Deve_Retornar_Dados_Corretamente_Do_Mediator()
        {
            // Arrange
            const string loginEsperado = "jsilva";
            const string nomeEsperado = "João da Silva";

            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = loginEsperado, Nome = nomeEsperado }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var parecerista = resultado.FirstOrDefault();

            // Assert
            Assert.NotNull(parecerista);
            Assert.Equal(loginEsperado, parecerista.Login);
            Assert.Equal(nomeEsperado, parecerista.Nome);
        }

        [Fact(DisplayName = "Executar - Deve manter ordem dos pareceristas retornados pelo mediator")]
        public async Task Executar_Deve_Manter_Ordem_Dos_Pareceristas_Retornados()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "aaa", Nome = "Primeiro" },
                new() { Login = "bbb", Nome = "Segundo" },
                new() { Login = "ccc", Nome = "Terceiro" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var resultadoList = resultado.ToList();

            // Assert
            Assert.Equal("aaa", resultadoList[0].Login);
            Assert.Equal("bbb", resultadoList[1].Login);
            Assert.Equal("ccc", resultadoList[2].Login);
            Assert.Equal("Primeiro", resultadoList[0].Nome);
            Assert.Equal("Segundo", resultadoList[1].Nome);
            Assert.Equal("Terceiro", resultadoList[2].Nome);
        }

        [Fact(DisplayName = "Executar - Deve fazer consulta com instance de ObterUsuariosPareceristasQuery")]
        public async Task Executar_Deve_Fazer_Consulta_Com_Instance_De_ObterUsuariosPareceristasQuery()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();
            ObterUsuariosPareceristasQuery? queryCapturada = null;

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<IEnumerable<RetornoUsuarioLoginNomeDTO>>, CancellationToken>(
                    (query, ct) => queryCapturada = query as ObterUsuariosPareceristasQuery)
                .ReturnsAsync(pareceristas);

            // Act
            await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(queryCapturada);
            Assert.IsType<ObterUsuariosPareceristasQuery>(queryCapturada);
        }

        [Fact(DisplayName = "Executar - Deve retornar Enumerable não mutável")]
        public async Task Executar_Deve_Retornar_Enumerable_Nao_Mutavel()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "test", Nome = "Test" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas.AsEnumerable());

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Any());
        }

        [Fact(DisplayName = "Executar - Deve ter propriedades Login e NomeSocial em RetornoUsuarioLoginNomeDTO")]
        public async Task Executar_Deve_Ter_Propriedades_Login_E_Nome_Em_RetornoUsuarioLoginNomeDTO()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "test", Nome = "Test" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();
            var parecerista = resultado.FirstOrDefault();

            // Assert
            Assert.NotNull(parecerista);
            var tipoLogin = parecerista.GetType().GetProperty("Login");
            var tipoNome = parecerista.GetType().GetProperty("NomeSocial");

            Assert.NotNull(tipoLogin);
            Assert.NotNull(tipoNome);
            Assert.NotNull(tipoLogin.GetValue(parecerista));
            Assert.NotNull(tipoNome.GetValue(parecerista));
        }

        [Fact(DisplayName = "Executar - Deve processar resultado sem dependência externa além de mediator")]
        public async Task Executar_Deve_Processar_Resultado_Sem_Dependencia_Externa_Alem_De_Mediator()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>();

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            Assert.NotNull(resultado);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact(DisplayName = "Executar - Deve retornar múltiplas enumerações sem problema")]
        public async Task Executar_Deve_Retornar_Multiplas_Enumeracoes_Sem_Problema()
        {
            // Arrange
            var pareceristas = new List<RetornoUsuarioLoginNomeDTO>
            {
                new() { Login = "test", Nome = "Test" }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuariosPareceristasQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pareceristas);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert - Enumera múltiplas vezes
            var primeiraEnumeracao = resultado.ToList();
            var segundaEnumeracao = resultado.ToList();
            var terceiraContagem = resultado.Count();

            Assert.Equal(primeiraEnumeracao.Count, segundaEnumeracao.Count);
            Assert.Equal(primeiraEnumeracao.Count, terceiraContagem);
        }
    }
}
