using AutoMapper;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> repositorio;
        private readonly Mock<IMapper> mapper;

        private readonly CasoDeUsoListarCodafSuplementar casoDeUso;

        public CasoDeUsoListarCodafSuplementarTestes()
        {
            repositorio = new Mock<IRepositorioCodafSuplementar>();
            mapper = new Mock<IMapper>();

            casoDeUso = new CasoDeUsoListarCodafSuplementar(
                repositorio.Object,
                mapper.Object);
        }

        [Fact]
        public async Task Deve_listar_codaf_suplementar_com_sucesso()
        {
            var filtro = new FiltroCodafSuplementarDto
            {
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            var filtroRepositorio = new FiltroListagemResultadoCodafSuplementarDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            mapper.Setup(x =>
                    x.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro))
                .Returns(filtroRepositorio);

            var itensRepositorio = new List<ListagemResultadoCodafSuplementarDto>();

            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafSuplementarDto>
            {
                Itens = itensRepositorio,
                TotalRegistros = 25,
                TamanhoPagina = 10,
                PaginaAtual = 1
            };

            repositorio.Setup(x =>
                    x.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            var itensDto = new List<CodafSuplementarResumoDto>
        {
            new()
            {
                Id = 1,
                NomeTurma = "Turma",
                NomeAreaPromotora = "Área"
            }
        };

            mapper.Setup(x =>
                    x.Map<List<CodafSuplementarResumoDto>>(itensRepositorio))
                .Returns(itensDto);

            var resultado = await casoDeUso.ExecutarAsync(filtro);

            Assert.True(resultado.Sucesso);

            Assert.Single(resultado.Dados!.Items);
            Assert.Equal(25, resultado.Dados!.TotalRegistros);
            Assert.Equal(3, resultado.Dados!.TotalPaginas);

            mapper.Verify(x =>
                x.Map<FiltroListagemResultadoCodafSuplementarDto>(filtro),
                Times.Once);

            mapper.Verify(x =>
                x.Map<List<CodafSuplementarResumoDto>>(itensRepositorio),
                Times.Once);

            repositorio.Verify(x =>
                x.ObterListagemResultadoCodafSuplementarPorFiltroAsync(filtroRepositorio),
                Times.Once);
        }
    }
}
