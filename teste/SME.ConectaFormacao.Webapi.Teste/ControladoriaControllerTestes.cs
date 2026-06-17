using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class ControladoriaControllerTestes
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly CoordenadoriaController controller;

        public ControladoriaControllerTestes()
        {
            mediatorMock = new Mock<IMediator>();
            controller = new CoordenadoriaController();
        }

        [Fact]
        public async Task Deve_Retornar_Lista_De_Coordenadorias()
        {
            var retorno = new List<CoordenadoriaDto>
            {
                new()
                {
                    Id = 1,
                    Nome = "Coordenadoria Centro",
                    Sigla = "CC",
                    NomeComSigla = "CC - Coordenadoria Centro"
                }
            };

            mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<ObterCoordenadoriasSelectQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);

            var resultado = await controller.ObterSelectCoordenadorias(mediatorMock.Object);

            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(retorno, resultado);

            mediatorMock.Verify(x =>
                x.Send(
                    It.IsAny<ObterCoordenadoriasSelectQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Enviar_Query_Correta_Para_Mediator()
        {
            mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<ObterCoordenadoriasSelectQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            await controller.ObterSelectCoordenadorias(mediatorMock.Object);

            mediatorMock.Verify(x =>
                x.Send(
                    It.Is<ObterCoordenadoriasSelectQuery>(q => q != null),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Mediator_Retornar_Vazio()
        {
            mediatorMock
                .Setup(x => x.Send(
                    It.IsAny<ObterCoordenadoriasSelectQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var resultado = await controller.ObterSelectCoordenadorias(mediatorMock.Object);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}
