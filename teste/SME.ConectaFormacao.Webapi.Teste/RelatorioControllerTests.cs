using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Webapi.Controllers;
using FluentAssertions;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class RelatorioControllerTests
    {
        private readonly Mock<ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao> _casoDeUsoRelatorioInscritosPorFormacaoMock;
        private readonly RelatorioController _controller;
        private readonly Faker _faker;

        public RelatorioControllerTests()
        {
            var mocker = new AutoMocker();
            _casoDeUsoRelatorioInscritosPorFormacaoMock = mocker.GetMock<ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao>();
            _controller = mocker.CreateInstance<RelatorioController>();
            _faker = new();
        }

        [Fact]
        public async Task DadoResultadoComSucesso_QuandoGerarRelatorioInscritosPorFormacao_EntaoRetornaAccepted()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                NumeroHomologacao = _faker.Random.Long(1, 1000),
                NomeFormacao = _faker.Random.Word(),
                PropostaTurmaId = _faker.Random.Long(1, 1000),
                Formato = _faker.PickRandom<Formato>(),
                AreaPromotoraId = _faker.Random.Long(1, 1000),
                PeriodoDeRealizacaoInicial = _faker.Date.Past(),
                PeriodoDeRealizacaoFinal = _faker.Date.Future(),
                SituacaoProposta = _faker.PickRandom<SituacaoProposta>(),
                SituacaoInscricao = _faker.PickRandom<SituacaoInscricao>(),
                CargoPublicoAlvoId = _faker.Random.Long(1, 1000),
                FuncaoId = _faker.Random.Long(1, 1000)
            };
            _casoDeUsoRelatorioInscritosPorFormacaoMock
                .Setup(c => c.ExecutarAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            var result = await _controller.GerarRelatorioInscritosPorFormacao(filtro);
            // Assert
            var acceptedResult = Assert.IsType<AcceptedResult>(result);
            acceptedResult.StatusCode.Should().Be(202);
        }

        [Fact]
        public async Task DadoResultadoComFalha_QuandoGerarRelatorioInscritosPorFormacao_EntaoRetornaBadRequest()
        {
            // Arrange
            var filtro = new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                NumeroHomologacao = _faker.Random.Long(1, 1000),
                NomeFormacao = _faker.Random.Word(),
                PropostaTurmaId = _faker.Random.Long(1, 1000),
                Formato = _faker.PickRandom<Formato>(),
                AreaPromotoraId = _faker.Random.Long(1, 1000),
                PeriodoDeRealizacaoInicial = _faker.Date.Past(),
                PeriodoDeRealizacaoFinal = _faker.Date.Future(),
                SituacaoProposta = _faker.PickRandom<SituacaoProposta>(),
                SituacaoInscricao = _faker.PickRandom<SituacaoInscricao>(),
                CargoPublicoAlvoId = _faker.Random.Long(1, 1000),
                FuncaoId = _faker.Random.Long(1, 1000)
            };
            var mensagemErro = "Erro ao gerar relatório";
            _casoDeUsoRelatorioInscritosPorFormacaoMock
                .Setup(c => c.ExecutarAsync(It.IsAny<FiltroRelatorioInscritosPorFormacaoDto>()))
                .ReturnsAsync(Erro.Validacao(mensagemErro));
            // Act
            var result = await _controller.GerarRelatorioInscritosPorFormacao(filtro);
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.StatusCode.Should().Be(400);
        }
    }
}
