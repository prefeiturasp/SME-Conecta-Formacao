using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Coordenadorias.ObterCoordenadoriasPaginado;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterCoordenadoriasPaginadoQueryHandlerTestes
    {
        private readonly Mock<IRepositorioCoordenadoria> _repositorioCoordenadoriaMock;
        private readonly ObterCoordenadoriasPaginadoQueryHandler _sut;
        private readonly Faker _faker;

        public ObterCoordenadoriasPaginadoQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioCoordenadoriaMock = mocker.GetMock<IRepositorioCoordenadoria>();
            _sut = mocker.CreateInstance<ObterCoordenadoriasPaginadoQueryHandler>();
            _faker = new("pt_BR");
        }

        [Fact]
        public async Task DadoFiltrosValidos_QuandoChamarMetodo_EntaoRetornaResultadoSucessoComItens()
        {
            // Arrange
            var query = new ObterCoordenadoriasPaginadoQuery(
                Nome: _faker.Company.CompanyName(),
                Sigla: _faker.Company.CompanySuffix(),
                Pagina: 1,
                TamanhoPagina: 10
            );

            var coordenadorias = new List<Coordenadoria>
            {
                new() { Id = _faker.Random.Long(1, 100), Nome = _faker.Company.CompanyName(), Sigla = _faker.Company.CompanySuffix() },
                new() { Id = _faker.Random.Long(101, 200), Nome = _faker.Company.CompanyName(), Sigla = _faker.Company.CompanySuffix() }
            };

            var resultadoPaginado = new ResultadoPaginado<Coordenadoria>
            {
                Itens = coordenadorias,
                TotalRegistros = 2,
                PaginaAtual = query.Pagina,
                TamanhoPagina = query.TamanhoPagina
            };

            _repositorioCoordenadoriaMock
                .Setup(r => r.ObterCoordenadoriaPaginadoAsync(query.Nome, query.Sigla, query.Pagina, query.TamanhoPagina))
                .ReturnsAsync(resultadoPaginado);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(resultadoPaginado.TotalRegistros);
            resultado.Dados.Items.Should().HaveCount(coordenadorias.Count);

            var itensRetornados = resultado.Dados.Items.ToList();
            for (int i = 0; i < coordenadorias.Count; i++)
            {
                itensRetornados[i].Id.Should().Be(coordenadorias[i].Id);
                itensRetornados[i].Nome.Should().Be(coordenadorias[i].Nome);
                itensRetornados[i].Sigla.Should().Be(coordenadorias[i].Sigla);
            }

            _repositorioCoordenadoriaMock
                .Verify(r => r.ObterCoordenadoriaPaginadoAsync(query.Nome, query.Sigla, query.Pagina, query.TamanhoPagina)
            , Times.Once);
        }

        [Fact]
        public async Task DadoFiltrosSemResultados_QuandoChamarMetodo_EntaoRetornaResultadoSucessoComListaVazia()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var query = new ObterCoordenadoriasPaginadoQuery(
                Nome: faker.Lorem.Word(),
                Sigla: faker.Lorem.Letter(),
                Pagina: 1,
                TamanhoPagina: 10
            );

            var resultadoPaginadoVazio = new ResultadoPaginado<Coordenadoria>
            {
                Itens = [],
                TotalRegistros = 0,
                PaginaAtual = query.Pagina,
                TamanhoPagina = query.TamanhoPagina
            };

            _repositorioCoordenadoriaMock
                .Setup(r => r.ObterCoordenadoriaPaginadoAsync(query.Nome, query.Sigla, query.Pagina, query.TamanhoPagina))
                .ReturnsAsync(resultadoPaginadoVazio);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(0);
            resultado.Dados.Items.Should().BeEmpty();

            _repositorioCoordenadoriaMock
                .Verify(r => r.ObterCoordenadoriaPaginadoAsync(query.Nome, query.Sigla, query.Pagina, query.TamanhoPagina)
                , Times.Once);
        }
    }
}
