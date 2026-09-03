using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoListarInscritosTurmaCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> repositorioCodafInscritosListaPresencaMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CasoDeUsoListarInscritosTurmaCodafListaPresenca casoDeUsoListarInscritosTurmaCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoListarInscritosTurmaCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            repositorioCodafInscritosListaPresencaMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            mapperMock = mocker.GetMock<IMapper>();
            casoDeUsoListarInscritosTurmaCodafListaPresenca = mocker.CreateInstance<CasoDeUsoListarInscritosTurmaCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1, 1000);
            repositorioCodafInscritosListaPresencaMock
                .Setup(r => r.ObterInscritosPorTurmaAsync(propostaTurmaId, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResultadoPaginado<ResultadoInscritoTurmaCodafListaPresencaDto>
                {
                    Itens = [],
                    PaginaAtual = 1,
                    TamanhoPagina = 10,
                    TotalRegistros = 0
                });
            var inscritosDto = new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>(
                [], 0, 1);
            mapperMock.Setup(m => m.Map<List<CodafInscritoTurmaListaPresencaRetornoDto>>(It.IsAny<IEnumerable<ResultadoInscritoTurmaCodafListaPresencaDto>>()))
                .Returns([]);

            // Act
            var resultado = await casoDeUsoListarInscritosTurmaCodafListaPresenca.ExecutarAsync(propostaTurmaId, 1, 10);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Should().BeEquivalentTo(inscritosDto);
        }
    }
}
