using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Ano;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ano;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class AnoTurmaControllerTestes
    {
        private readonly Mock<ICasoDeUsoObterListaAnoTurma> _mockCasoDeUso;
        private readonly AnoTurmaController _sut;

        public AnoTurmaControllerTestes()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUso = mocker.GetMock<ICasoDeUsoObterListaAnoTurma>();
            _sut = mocker.CreateInstance<AnoTurmaController>();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterAnoPorModalidadeAnoLetivo_EntaoRetornaListaComSucesso()
        {
            // Arrange
            var filtro = new FiltroAnoTurmaDTO();
            var retornoDesejado = new List<RetornoListagemTodosDTO>
            {
                new() { Id = 1, Descricao = "Teste" }
            };

            _mockCasoDeUso
                .Setup(m => m.Executar(filtro))
                .ReturnsAsync(retornoDesejado);

            // Act
            var resultado = await _sut.ObterAnoPorModalidadeAnoLetivo(_mockCasoDeUso.Object, filtro) as OkObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(retornoDesejado);

            _mockCasoDeUso.Verify(m => m.Executar(filtro), Times.Once);
        }
    }
}
