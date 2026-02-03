using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoListarCodafListaPresenca _casoDeUsoListarCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoListarCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _mapperMock = mocker.GetMock<IMapper>();
            _casoDeUsoListarCodafListaPresenca = mocker.CreateInstance<CasoDeUsoListarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var filtroRepositorioDto = new FiltroListagemResultadoCodafListaPresencaDto
            {
                NomeFormacao = filtroDto.NomeFormacao,
                CodigoFormacao = filtroDto.CodigoFormacao.ToString(),
                Pagina = filtroDto.NumeroPagina,
                TamanhoPagina = filtroDto.NumeroRegistros
            };
            var resultadoRepositorio = new ResultadoPaginado<ListagemResultadoCodafListaPresencaDto>
            {
                Itens = [],
                TotalRegistros = 0
            };
            var resultadoEsperado = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>([], 0, 0);
            _mapperMock.Setup(m => m.Map<FiltroListagemResultadoCodafListaPresencaDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _repositorioCodafListaPresencaMock.Setup(r => r.ObterListagemResultadoCodafListaPresencaPorFiltroAsync(filtroRepositorioDto))
                .ReturnsAsync(resultadoRepositorio);
            _mapperMock.Setup(m => m.Map<List<ListaPresencaCodafResumoDto>>(resultadoRepositorio.Itens))
                .Returns([]);

            // Act
            var resultado = await _casoDeUsoListarCodafListaPresenca.ExecutarAsync(filtroDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().BeEquivalentTo(resultadoEsperado);
        }
    }
}
