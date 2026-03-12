using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Ues;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAutocompletarNomeUeTeste
    {
        private readonly Mock<IRepositorioUe> _repositorioUeMock;
        private readonly CasoDeUsoObterAutocompletarNomeUe _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterAutocompletarNomeUeTeste()
        {
            var mocker = new AutoMocker();
            _repositorioUeMock = mocker.GetMock<IRepositorioUe>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterAutocompletarNomeUe>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmTermoBuscaVazio_QuandoChamarExecutarAsync_EntaoDeveRetornarDadosVazio()
        {
            // Arrange
            var filtro = new FiltroAutocompletarNomeUeDto
            {
                TermoBusca = "",
                DreId = 1,
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoUmTermoBuscaValido_QuandoChamarExecutarAsync_EntaoDeveRetornarDados()
        {
            // Arrange
            var filtro = new FiltroAutocompletarNomeUeDto
            {
                TermoBusca = _faker.Random.Word(),
                DreId = 1,
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var itensMock = new List<AutocompletarNomeUeDto>
            {
                new() { Id = Guid.NewGuid(), Nome = "UE 1" },
                new() { Id = Guid.NewGuid(), Nome = "UE 2" }
            };
            var resultadoMock = new ResultadoPaginado<AutocompletarNomeUeDto>
            {
                Itens = itensMock,
                TotalRegistros = itensMock.Count,
                PaginaAtual = filtro.NumeroPagina,
                TamanhoPagina = filtro.NumeroRegistros
            };
            _repositorioUeMock.Setup(r => r.AutocompletarNomeAsync(filtro.TermoBusca, filtro.DreId, filtro.NumeroPagina, filtro.NumeroRegistros))
                .ReturnsAsync(resultadoMock);
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Items.Should().HaveCount(itensMock.Count);
        }

    }
}
