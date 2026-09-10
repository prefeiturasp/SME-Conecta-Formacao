using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoListarMinhasDeclaracoesCodafTestes
    {
        private readonly Mock<IRepositorioCodafDeclaracao> _repositorioCodafDeclaracaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Faker _faker;
        private readonly CasoDeUsoListarMinhasDeclaracoesCodaf _sut;

        public CasoDeUsoListarMinhasDeclaracoesCodafTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafDeclaracaoMock = mocker.GetMock<IRepositorioCodafDeclaracao>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoListarMinhasDeclaracoesCodaf>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutar_EntaoDeveRetornarPaginacaoComDeclaracoes()
        {
            // Arrange
            var filtro = new FiltroListaMinhasDeclaracoesCodafDto { NumeroPagina = 1, NumeroRegistros = 10 };
            var filtroRepositorio = new FiltroMinhasDeclaracoesCodafDto { Pagina = 1, TamanhoPagina = 10 };
            var declaracaoDto = new MinhasDeclaracoesCodafDto();
            var itens = new List<MinhasDeclaracoesCodafDto> { declaracaoDto };
            // O repositório retorna ResultadoPaginado<T> (infra.dados), não PaginacaoResultadoDto
            var resultadoRepositorio = new ResultadoPaginado<MinhasDeclaracoesCodafDto>
            {
                Itens = itens,
                TotalRegistros = 1,
                TamanhoPagina = 10
            };

            _mapperMock
                .Setup(m => m.Map<FiltroMinhasDeclaracoesCodafDto>(filtro))
                .Returns(filtroRepositorio);

            _repositorioCodafDeclaracaoMock
                .Setup(r => r.ObterMinhasDeclaracoesPorFiltroAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            // PaginacaoResultadoDto<T> usa Items (não Itens)
            resultado.Dados!.Items.Should().HaveCount(1);
            resultado.Dados.Items.Should().Contain(declaracaoDto);
            resultado.Dados.TotalRegistros.Should().Be(1);
            
            _mapperMock.Verify(m => m.Map<FiltroMinhasDeclaracoesCodafDto>(filtro), Times.Once);
            _repositorioCodafDeclaracaoMock.Verify(r => r.ObterMinhasDeclaracoesPorFiltroAsync(filtroRepositorio), Times.Once);
        }
    }
}
