using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Dre;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterDreListaUsuarioLogadoTestes
    {
        private readonly Mock<IMediator> mediator;

        public CasoDeUsoObterDreListaUsuarioLogadoTestes()
        {
            mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Deve_Obter_Lista_De_Dres_Do_Usuario_Logado()
        {
            // Arrange
            var dres = new List<Dominio.Entidades.Dre>
            {
                new()
                {
                    Id = 1,
                    Nome = "DRE Butantã"
                },
                new()
                {
                    Id = 2,
                    Nome = "DRE Campo Limpo"
                }
            };

            mediator
                .Setup(m => m.Send(
                    It.IsAny<ObterDresPorGrupoUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(dres);

            var casoDeUso = new CasoDeUsoObterDreListaUsuarioLogado(mediator.Object);

            // Act
            var retorno = (await casoDeUso.Executar()).ToList();

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(2, retorno.Count);

            Assert.Equal(dres[0].Id, retorno[0].Id);
            Assert.Equal(dres[0].Nome, retorno[0].Descricao);

            Assert.Equal(dres[1].Id, retorno[1].Id);
            Assert.Equal(dres[1].Nome, retorno[1].Descricao);

            mediator.Verify(
                m => m.Send(
                    It.Is<ObterDresPorGrupoUsuarioLogadoQuery>(
                        query => query == ObterDresPorGrupoUsuarioLogadoQuery.Instancia()),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Existirem_Dres()
        {
            // Arrange
            var dres = Enumerable.Empty<Dominio.Entidades.Dre>();

            mediator
                .Setup(m => m.Send(
                    It.IsAny<ObterDresPorGrupoUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(dres);

            var casoDeUso = new CasoDeUsoObterDreListaUsuarioLogado(mediator.Object);

            // Act
            var retorno = await casoDeUso.Executar();

            // Assert
            Assert.NotNull(retorno);
            Assert.Empty(retorno);

            mediator.Verify(
                m => m.Send(
                    It.IsAny<ObterDresPorGrupoUsuarioLogadoQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
