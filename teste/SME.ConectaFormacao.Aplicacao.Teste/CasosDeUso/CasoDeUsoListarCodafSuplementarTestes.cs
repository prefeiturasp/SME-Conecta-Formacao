using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafSuplementarTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoListarCodafSuplementar _sut;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafSuplementarTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoListarCodafSuplementar>();
            _faker = new Faker("pt_BR");
        }
        [Fact]
        public async Task DadoFiltroValido_QuandoExecutarAsync_EntaoDeveRetornarListagemPaginadaComSucesso()
        {
            // Arrange
            var filtro = new Faker<FiltroCodafSuplementarDto>("pt_BR")
                .RuleFor(f => f.NumeroPagina, f => f.Random.Int(1, 10))
                .RuleFor(f => f.NumeroRegistros, f => f.Random.Int(10, 50))
                .Generate();

            var filtroRepositorio = new Faker<FiltroListagemResultadoCodafSuplementarDto>("pt_BR")
                .RuleFor(f => f.Pagina, filtro.NumeroPagina)
                .RuleFor(f => f.TamanhoPagina, filtro.NumeroRegistros)
                .Generate();

            var listagemResultadoRepositorio = new Faker<ListagemResultadoCodafSuplementarDto>("pt_BR").Generate(5);

            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafSuplementarDto>
            {
                Itens = listagemResultadoRepositorio,
                TotalRegistros = _faker.Random.Int(100, 500),
                PaginaAtual = filtro.NumeroPagina,
                TamanhoPagina = filtro.NumeroRegistros
            };

            var listagemResumoDto = new Faker<CodafSuplementarResumoDto>("pt_BR")
                .RuleFor(c => c.Id, f => f.Random.Long(1))
                .RuleFor(c => c.NomeTurma, f => f.Random.Word())
                .RuleFor(c => c.NomeAreaPromotora, f => f.Company.CompanyName())
                .Generate(5);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafSuplementar>()
                .Setup(r => r.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<List<CodafSuplementarResumoDto>>(resultadoRepositorio.Itens))
                .Returns(listagemResumoDto);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(resultadoRepositorio.TotalRegistros);
            resultado.Dados.Items.Should().HaveCount(5);
            resultado.Dados.Items.Should().BeEquivalentTo(listagemResumoDto);

            _mocker.GetMock<IMapper>().Verify(m => m.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro), Times.Once);
            _mocker.GetMock<IRepositorioCodafSuplementar>().Verify(r => r.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio), Times.Once);
            _mocker.GetMock<IMapper>().Verify(m => m.Map<List<CodafSuplementarResumoDto>>(resultadoRepositorio.Itens), Times.Once);
        }

        [Fact]
        public async Task DadoNenhumRegistroEncontrado_QuandoExecutarAsync_EntaoDeveRetornarListagemPaginadaVazia()
        {
            // Arrange
            var filtro = new Faker<FiltroCodafSuplementarDto>("pt_BR")
                .RuleFor(f => f.NumeroPagina, 1)
                .RuleFor(f => f.NumeroRegistros, 10)
                .Generate();

            var filtroRepositorio = new Faker<FiltroListagemResultadoCodafSuplementarDto>("pt_BR")
                .RuleFor(f => f.Pagina, 1)
                .RuleFor(f => f.TamanhoPagina, 10)
                .Generate();

            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafSuplementarDto>
            {
                Itens = [],
                TotalRegistros = 0,
                PaginaAtual = 1,
                TamanhoPagina = 10
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafSuplementar>()
                .Setup(r => r.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<List<CodafSuplementarResumoDto>>(resultadoRepositorio.Itens))
                .Returns([]);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(0);
            resultado.Dados.Items.Should().BeEmpty();

            _mocker.GetMock<IMapper>().Verify(m => m.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro), Times.Once);
            _mocker.GetMock<IRepositorioCodafSuplementar>().Verify(r => r.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio), Times.Once);
            _mocker.GetMock<IMapper>().Verify(m => m.Map<List<CodafSuplementarResumoDto>>(resultadoRepositorio.Itens), Times.Once);
        }
    }
}
