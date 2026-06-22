using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafConsultaAuxiliarControllerTests
    {
        private readonly Mock<ICasoDeUsoListarInscritosTurmaCodafListaPresenca> _mockCasoDeUsoListarInscritosTurma;
        private readonly Mock<ICasoDeUsoTurmaPossuiCodafListaPresenca> _mockCasoDeUsoTurmaPossuiCodafListaPresenca;
        private readonly Mock<ICasoDeUsoObterPropostaTurmaComCodaf> _mockCasoDeUsoObterPropostaTurmaComCodaf;
        private readonly CodafConsultaAuxiliarController _controller;
        private readonly Faker _faker;

        public CodafConsultaAuxiliarControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoListarInscritosTurma = mocker.GetMock<ICasoDeUsoListarInscritosTurmaCodafListaPresenca>();
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca = mocker.GetMock<ICasoDeUsoTurmaPossuiCodafListaPresenca>();
            _mockCasoDeUsoObterPropostaTurmaComCodaf = mocker.GetMock<ICasoDeUsoObterPropostaTurmaComCodaf>();
            _controller = mocker.CreateInstance<CodafConsultaAuxiliarController>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarObterInscritosPorTurma_EntaoDeveChamarCasoDeUsoListarInscritosTurma()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var numeroPagina = 1;
            var numeroRegistros = 10;
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(
                    new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0)));
            // Act
            await _controller.ObterInscritosPorTurma(propostaTurmaId, numeroPagina, numeroRegistros);
            // Assert
            _mockCasoDeUsoListarInscritosTurma.Verify(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros), Times.Once);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarObterInscritosPorTurma_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var numeroPagina = 1;
            var numeroRegistros = 10;
            var paginacaoResultadoDto = new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0);
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(paginacaoResultadoDto));
            // Act
            var resultado = await _controller.ObterInscritosPorTurma(propostaTurmaId, numeroPagina, numeroRegistros) as ObjectResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultadoValor = resultado.Value as PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(paginacaoResultadoDto);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarTurmaPossuiListaPresenca_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, 1, 10))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(
                    new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0)));
            // Act
            await _controller.ObterInscritosPorTurma(propostaTurmaId);
            // Assert
            _mockCasoDeUsoListarInscritosTurma.Verify(x => x.ExecutarAsync(propostaTurmaId, 1, 10), Times.Once);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarTurmaPossuiListaPresenca_EntaoDeveChamarCasoDeUsoTurmaPossuiCodafListaPresenca()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca
                .Setup(x => x.ExecutarAsync(propostaTurmaId))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.TurmaPossuiListaPresenca(propostaTurmaId);
            // Assert
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca.Verify(x => x.ExecutarAsync(propostaTurmaId), Times.Once);
        }

        [Fact]
        public async Task DadoUmaPropostaId_QuandoObterTurmasComCodafPorProposta_EntaoDeveChamarCasoDeUsoObterPropostaTurmaComCodaf()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            _mockCasoDeUsoObterPropostaTurmaComCodaf
                .Setup(x => x.ExecutarAsync(propostaId))
                .ReturnsAsync(Resultado<IEnumerable<PropostaTurmaComCodafDto>>.DeSucesso([]));
            // Act
            await _controller.ObterTurmasComCodafPorProposta(propostaId);
            // Assert
            _mockCasoDeUsoObterPropostaTurmaComCodaf.Verify(x => x.ExecutarAsync(propostaId), Times.Once);
        }
    }
}
