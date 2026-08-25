using MediatR;
using Moq;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDresPorGrupoUsuarioLogadoQueryHandlerTestes
    {
        private readonly Mock<IMediator> mediator;
        private readonly Mock<IRepositorioAreaPromotora> repositorioAreaPromotora;

        public ObterDresPorGrupoUsuarioLogadoQueryHandlerTestes()
        {
            mediator = new Mock<IMediator>();
            repositorioAreaPromotora = new Mock<IRepositorioAreaPromotora>();
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Mediator_For_Nulo()
        {
            // Act
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ObterDresPorGrupoUsuarioLogadoQueryHandler(
                    null!,
                    repositorioAreaPromotora.Object));

            // Assert
            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public void Deve_Lancar_ArgumentNullException_Quando_Repositorio_Area_Promotora_For_Nulo()
        {
            // Act
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ObterDresPorGrupoUsuarioLogadoQueryHandler(
                    mediator.Object,
                    null!));

            // Assert
            Assert.Equal("repositorioAreaPromotora", excecao.ParamName);
        }

        [Fact]
        public async Task Deve_Obter_Dres_Pelo_Grupo_Do_Usuario_Logado()
        {
            // Arrange

            var grupoId = new Guid();

            var dres = new List<Dre>
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

            var cancellationToken = CancellationToken.None;

            mediator
                .Setup(m => m.Send(
                    It.IsAny<ObterGrupoUsuarioLogadoQuery>(),
                    cancellationToken))
                .ReturnsAsync(grupoId);

            repositorioAreaPromotora
                .Setup(r => r.ObterDresPorGrupoId(grupoId))
                .ReturnsAsync(dres);

            var handler = new ObterDresPorGrupoUsuarioLogadoQueryHandler(
                mediator.Object,
                repositorioAreaPromotora.Object);

            var request = ObterDresPorGrupoUsuarioLogadoQuery.Instancia();

            // Act
            var retorno = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(dres, retorno);

            mediator.Verify(
                m => m.Send(
                    It.Is<ObterGrupoUsuarioLogadoQuery>(
                        query => query == ObterGrupoUsuarioLogadoQuery.Instancia()),
                    cancellationToken),
                Times.Once);

            repositorioAreaPromotora.Verify(
                r => r.ObterDresPorGrupoId(grupoId),
                Times.Once);
        }
    }
}
