using AutoMapper;
using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCertificados;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarTodosCertificadosCodafTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoListarTodosCertificadosCodaf _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoListarTodosCertificadosCodafTestes()
        {
            _mocker = new AutoMocker();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoListarTodosCertificadosCodaf>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutarAsync_EntaoDeveRetornarCertificadosPaginadosComSucesso()
        {
            // Arrange
            var filtro = new FiltroListaTodosCertificadosCodafDto
            {
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            var filtroRepositorio = new FiltroListagemTodosCertificadosCodafDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            var certificados = new List<ListagemCertificadosCodafDto>
            {
                new()
                {
                    Id = _faker.Random.Long(1, 1000),
                    NomeParticipante = _faker.Person.FullName,
                    TipoCertificado = TipoCertificadoCodaf.Cursista
                },
                new()
                {
                    Id = _faker.Random.Long(1, 1000),
                    NomeParticipante = _faker.Person.FullName,
                    TipoCertificado = TipoCertificadoCodaf.Regente
                }
            };

            var resultadoRepositorio = new ResultadoPaginado<ListagemCertificadosCodafDto>
            {
                Itens = certificados,
                TotalRegistros = 2,
                TamanhoPagina = 10
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemTodosCertificadosCodafDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(r => r.ObterTodosCertificadosAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(2, resultado.Dados.TotalRegistros);
            Assert.Equal(2, resultado.Dados.Items.Count());

            _mocker.GetMock<IMapper>().Verify(m => m.Map<FiltroListagemTodosCertificadosCodafDto>(filtro), Times.Once);
            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(r => r.ObterTodosCertificadosAsync(filtroRepositorio), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoNaoHouverCertificadosNoRepositorio_EntaoDeveRetornarPaginacaoVaziaComSucesso()
        {
            // Arrange
            var filtro = new FiltroListaTodosCertificadosCodafDto
            {
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            var filtroRepositorio = new FiltroListagemTodosCertificadosCodafDto
            {
                Pagina = 1,
                TamanhoPagina = 10
            };

            var resultadoRepositorio = new ResultadoPaginado<ListagemCertificadosCodafDto>
            {
                Itens = Enumerable.Empty<ListagemCertificadosCodafDto>(),
                TotalRegistros = 0,
                TamanhoPagina = 10
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<FiltroListagemTodosCertificadosCodafDto>(filtro))
                .Returns(filtroRepositorio);

            _mocker.GetMock<IRepositorioCodafCertificado>()
                .Setup(r => r.ObterTodosCertificadosAsync(filtroRepositorio))
                .ReturnsAsync(resultadoRepositorio);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(filtro);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Equal(0, resultado.Dados.TotalRegistros);
            Assert.Empty(resultado.Dados.Items);

            _mocker.GetMock<IRepositorioCodafCertificado>().Verify(r => r.ObterTodosCertificadosAsync(filtroRepositorio), Times.Once);
        }
    }
}