using AutoMapper;
using Bogus;
using FluentAssertions;
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
    public class CasoDeUsoListarMeusCertificadosCodafTests
    {
        private readonly Mock<IRepositorioCodafCertificado> _mockRepositorio;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CasoDeUsoListarMeusCertificadosCodaf _sut;
        private readonly Faker _faker;

        public CasoDeUsoListarMeusCertificadosCodafTests()
        {
            var mocker = new AutoMocker();
            _mockRepositorio = mocker.GetMock<IRepositorioCodafCertificado>();
            _mockMapper = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoListarMeusCertificadosCodaf>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmFiltroValido_QuandoExecutarAsync_EntaoDeveRetornarResultadoEsperado()
        {
            // Arrange
            var filtroDto = new FiltroListaMeusCertificadosCodafDto
            {
                NumeroPagina = 1,
                NumeroRegistros = 10,
                NumeroHomologacao = _faker.Random.Int(1000, 9999).ToString(),
                NomeFormacao = _faker.Lorem.Word(),
                CodigoCertificado = _faker.Random.Long(1000, 9999),
                TipoParticipacao = _faker.PickRandom<TipoParticipacaoCodaf>(),
                DataEmissaoInicio = _faker.Date.Past(),
                DataEmissaoFim = _faker.Date.Recent()
            };
            var filtroRepositorioDto = new FiltroMeusCertificadosCodafDto
            {
                Pagina = filtroDto.NumeroPagina,
                TamanhoPagina = filtroDto.NumeroRegistros,
                NumeroHomologacao = filtroDto.NumeroHomologacao,
                NomeFormacao = filtroDto.NomeFormacao,
                CodigoCertificado = filtroDto.CodigoCertificado,
                TipoParticipacao = filtroDto.TipoParticipacao,
                DataEmissaoInicio = filtroDto.DataEmissaoInicio,
                DataEmissaoFim = filtroDto.DataEmissaoFim
            };
            var certificadosRepositorio = new ResultadoPaginado<MeusCertificadosCodafDto>
            {
                Itens =
                [
                    new MeusCertificadosCodafDto
                    {
                        Id = 1,
                        NumeroHomologacao = 1234,
                        NomeFormacao = "Formação Exemplo",
                        CodigoCertificado = 5678,
                        TemRf = true,
                        TipoParticipacao = TipoParticipacaoCodaf.Cursista,
                        DataEmissao = DateTime.Now.AddDays(-10)
                    }
                ],
                TotalRegistros = 1,
                TamanhoPagina = 10
            };
            _mockMapper
                .Setup(m => m.Map<FiltroMeusCertificadosCodafDto>(filtroDto))
                .Returns(filtroRepositorioDto);
            _mockRepositorio
                .Setup(r => r.ObterMeusCertificadosPorFiltroAsync(filtroRepositorioDto))
                .ReturnsAsync(certificadosRepositorio);

            // Act
            var resultado = await _sut.ExecutarAsync(filtroDto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.TotalRegistros.Should().Be(1);
            _mockMapper.VerifyAll();
            _mockRepositorio.VerifyAll();
        }
    }
}
