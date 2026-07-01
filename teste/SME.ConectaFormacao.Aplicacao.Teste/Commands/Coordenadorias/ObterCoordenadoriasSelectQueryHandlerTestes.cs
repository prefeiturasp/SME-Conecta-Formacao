using Moq;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasSelect;
using SME.ConectaFormacao.Infra.Dados.Dtos.Coordenadorias;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Coordenadorias
{
    public class ObterCoordenadoriasSelectQueryHandlerTests
    {
        private readonly Mock<IRepositorioCoordenadoria> repositorioMock;
        private readonly ObterCoordenadoriasSelectQueryHandler handler;

        public ObterCoordenadoriasSelectQueryHandlerTests()
        {
            repositorioMock = new Mock<IRepositorioCoordenadoria>();
            handler = new ObterCoordenadoriasSelectQueryHandler(repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_Chamar_Repositorio_Uma_Unica_Vez()
        {
            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync([]);

            var query = new ObterCoordenadoriasSelectQuery();

            await handler.Handle(query, CancellationToken.None);

            repositorioMock.Verify(
                x => x.ObterCoordenadoriaSelectAsync(),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Mapear_Corretamente_Coordenadorias_Com_Sigla()
        {
            var coordenadorias = new List<CoordenadoriaDto>
            {
                new()
                {
                    Id = 1,
                    Nome = "Coordenadoria Norte",
                    Sigla = "DN"
                }
            };

            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync(coordenadorias);

            var resultado = await handler.Handle(
                new ObterCoordenadoriasSelectQuery(),
                CancellationToken.None);

            Assert.Single(resultado);

            var dto = resultado.First();

            Assert.Equal(1, dto.Id);
            Assert.Equal("Coordenadoria Norte", dto.Nome);
            Assert.Equal("DN", dto.Sigla);
            Assert.Equal("DN - Coordenadoria Norte", dto.NomeComSigla);
        }

        [Fact]
        public async Task Deve_Usar_Apenas_Nome_Quando_Sigla_For_Nula()
        {
            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync(
                [
                    new()
                    {
                        Id = 10,
                        Nome = "Centro",
                        Sigla = null
                    }
                ]);

            var resultado = await handler.Handle(
                new ObterCoordenadoriasSelectQuery(),
                CancellationToken.None);

            Assert.Single(resultado);
            Assert.Equal("Centro", resultado[0].NomeComSigla);
        }

        [Fact]
        public async Task Deve_Usar_Apenas_Nome_Quando_Sigla_For_Vazia()
        {
            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync(
                [
                    new()
                    {
                        Id = 20,
                        Nome = "Sul",
                        Sigla = string.Empty
                    }
                ]);

            var resultado = await handler.Handle(
                new ObterCoordenadoriasSelectQuery(),
                CancellationToken.None);

            Assert.Single(resultado);
            Assert.Equal("Sul", resultado[0].NomeComSigla);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Repositorio_Nao_Retornar_Registros()
        {
            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync([]);

            var resultado = await handler.Handle(
                new ObterCoordenadoriasSelectQuery(),
                CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task Deve_Mapear_Todos_Os_Registros()
        {
            repositorioMock
                .Setup(x => x.ObterCoordenadoriaSelectAsync())
                .ReturnsAsync(
                [
                    new()
                    {
                        Id = 1,
                        Nome = "Centro",
                        Sigla = "CE"
                    },
                    new()
                    {
                        Id = 2,
                        Nome = "Sul",
                        Sigla = null
                    },
                    new()
                    {
                        Id = 3,
                        Nome = "Leste",
                        Sigla = ""
                    }
                ]);

            var resultado = await handler.Handle(
                new ObterCoordenadoriasSelectQuery(),
                CancellationToken.None);

            Assert.Equal(3, resultado.Count);

            Assert.Equal("CE - Centro", resultado[0].NomeComSigla);
            Assert.Equal("Sul", resultado[1].NomeComSigla);
            Assert.Equal("Leste", resultado[2].NomeComSigla);
        }
    }
}
