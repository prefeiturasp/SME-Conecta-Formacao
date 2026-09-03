using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafCursoNaoHomologadoTeste
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoListarCodafCursoNaoHomologado _sut;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafCursoNaoHomologadoTeste()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoListarCodafCursoNaoHomologado>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutarAsync_EntaoDeveRetornarListagemPaginadaComSucesso()
        {
            // Arrange
            var filtro = new Faker<FiltroCodafCursoNaoHomologadoDto>("pt_BR")
                .RuleFor(f => f.NumeroPagina, f => f.Random.Int(1, 10))
                .RuleFor(f => f.NumeroRegistros, f => f.Random.Int(10, 50))
                .Generate();

            var filtroRepositorio = _mocker.Get<FiltroListagemResultadoCodafCursoNaoHomologadoDto>() ?? new Faker<FiltroListagemResultadoCodafCursoNaoHomologadoDto>().Generate();

            var listagemResultadoRepositorio = new Faker<ListagemResultadoCodafCursoNaoHomologadoDto>("pt_BR")
                .RuleFor(c => c.Id, f => f.Random.Long(1))
                .RuleFor(c => c.NomeTurma, f => f.Random.Word())
                .RuleFor(c => c.NomeAreaPromotora, f => f.Company.CompanyName())
                .Generate(5);

            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>
            {
                Itens = listagemResultadoRepositorio,
                TotalRegistros = _faker.Random.Int(100, 500),
                PaginaAtual = filtro.NumeroPagina,
                TamanhoPagina = filtro.NumeroRegistros
            };

            var listagemResumoDto = new Faker<CodafCursoNaoHomologadoResumoDto>("pt_BR")
                .RuleFor(c => c.Id, f => f.Random.Long(1))
                .RuleFor(c => c.CodigoFormacao, f => f.Random.Long(1))
                .RuleFor(c => c.NomeTurma, f => f.Random.Word())
                .Generate(5);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultadoRepositorio.Itens))
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

            _mocker.GetMock<IMapper>().Verify(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro), Times.Once);
            _mocker.GetMock<IRepositorioCodafCursoNaoHomologado>().Verify(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio), Times.Once);
            _mocker.GetMock<IMapper>().Verify(m => m.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultadoRepositorio.Itens), Times.Once);
        }

        [Fact]
        public async Task DadoNenhumRegistroEncontrado_QuandoExecutarAsync_EntaoDeveRetornarListagemPaginadaVazia()
        {
            // Arrange
            var filtro = new Faker<FiltroCodafCursoNaoHomologadoDto>("pt_BR")
                .RuleFor(f => f.NumeroPagina, 1)
                .RuleFor(f => f.NumeroRegistros, 10)
                .Generate();

            var filtroRepositorio = _mocker.Get<FiltroListagemResultadoCodafCursoNaoHomologadoDto>() ?? new Faker<FiltroListagemResultadoCodafCursoNaoHomologadoDto>().Generate();

            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>
            {
                Itens = new List<ListagemResultadoCodafCursoNaoHomologadoDto>(),
                TotalRegistros = 0,
                PaginaAtual = 1,
                TamanhoPagina = 10
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultadoRepositorio.Itens))
                .Returns(new List<CodafCursoNaoHomologadoResumoDto>());

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.TotalRegistros.Should().Be(0);
            resultado.Dados.Items.Should().BeEmpty();

            _mocker.GetMock<IMapper>().Verify(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro), Times.Once);
            _mocker.GetMock<IRepositorioCodafCursoNaoHomologado>().Verify(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio), Times.Once);
            _mocker.GetMock<IMapper>().Verify(m => m.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultadoRepositorio.Itens), Times.Once);
        }
    }
}
