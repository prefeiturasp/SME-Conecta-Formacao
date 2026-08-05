using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoListarCodafCursoNaoHomologado _sut;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoListarCodafCursoNaoHomologado>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarExecutar_EntaoDeveRetornarPaginacaoComSucesso()
        {
            // Arrange
            var filtro = new FiltroCodafCursoNaoHomologadoDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var filtroRepositorio = new FiltroListagemResultadoCodafCursoNaoHomologadoDto { Pagina = 1, TamanhoPagina = 10 };
            
            var itensRepositorio = new List<ListagemResultadoCodafCursoNaoHomologadoDto> 
            { 
                new ListagemResultadoCodafCursoNaoHomologadoDto { Id = _faker.Random.Long(1, 100), NomeTurma = "T1", NomeAreaPromotora = "A1" } 
            };
            var resultadoPaginadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafCursoNaoHomologadoDto>
            {
                Itens = itensRepositorio,
                TotalRegistros = 1,
                TamanhoPagina = 10
            };

            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro))
                       .Returns(filtroRepositorio);

            _repositorioMock.Setup(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio))
                            .ReturnsAsync(resultadoPaginadoRepositorio);

            _mapperMock.Setup(m => m.Map<List<CodafCursoNaoHomologadoResumoDto>>(resultadoPaginadoRepositorio.Itens))
                       .Returns(new List<CodafCursoNaoHomologadoResumoDto> { new CodafCursoNaoHomologadoResumoDto { Id = 1 } });

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Items.Should().HaveCount(1);
            resultado.Dados.TotalRegistros.Should().Be(1);
            
            _mapperMock.Verify(m => m.Map<FiltroListagemResultadoCodafCursoNaoHomologadoDto>(filtro), Times.Once);
            _repositorioMock.Verify(r => r.ObterListagemResultadoCodafCursoNaoHomologadoPorFiltroAsync(filtroRepositorio), Times.Once);
        }
    }
}
